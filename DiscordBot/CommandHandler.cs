using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Extensions;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IHelperService _helperService;
        private readonly ICommandService _commandService;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IResourceManager _resourceManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CommandHandler(
            IDiscordClientAccessor discordClientAccessor,
            IBlockingPriorityQueue blockingPriorityQueue,
            IHelperService helperService,
            ICommandService commandService,
            IOptionsMonitor<DiscordConfiguration> config,
            ILogger<CommandHandler> logger,
            ISystemClock systemClock,
            IResourceManager resourceManager,
            IServiceProvider serviceProvider,
            IServiceScopeFactory scopeFactory)
        {
            _discordClientAccessor = discordClientAccessor;
            _blockingPriorityQueue =  blockingPriorityQueue;
            _helperService = helperService;
            _commandService = commandService;
            _config = config;
            _logger = logger;
            _systemClock = systemClock;
            _resourceManager = resourceManager;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = scopeFactory;
        }

        public async Task InitializeAsync()
        {
            var client = _discordClientAccessor.Client;

            var modules = await _commandService
                .AddModulesAsync(typeof(CommandHandler).Assembly, _serviceProvider);
            _helperService.AddPublicModuleInfo(modules);

            _helperService.AddPrivateModuleInfo(
                (PrivateModules.Moderation, await _commandService.AddModuleAsync<Modules.ModerationModule>(_serviceProvider)),
                (PrivateModules.Debug, await _commandService.AddModuleAsync<Modules.DebugModule>(_serviceProvider)));

            _discordClientAccessor.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(IMessage message)
        {
#if DEBUG
            try
            {
#endif
                var userMessage = message as IUserMessage;

                if (userMessage == null)
                {
                    return;
                }

                var user = userMessage.Author;

                if (user.IsBotOrWebhook())
                {
                    return;
                }

                var config = _config.CurrentValue;
                var prefix = config.Prefix;

                var guildUser = userMessage.Author as IGuildUser;

                if (guildUser == null)
                {
                    return;
                }

                var guild = guildUser.Guild;
                var guildId = guild.Id;
                var client = _discordClientAccessor.Client;
                var channel = (IMessageChannel)await client.GetChannelAsync(userMessage.Channel.Id);

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
                var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

                if (guildConfig?.Prefix != null)
                {
                    prefix = guildConfig.Prefix;
                }

                var argumentPosition = 0;

                if (!userMessage.HasStringPrefix(prefix, ref argumentPosition, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var userId = guildUser.Id;
                var isDev = config.AllowedToDebug.Any(x => x == userId);
                var isOnBlacklist = config.BlacklistedGuilds.Any(x => x == guild.Id);

                if (isOnBlacklist && !isDev)
                {
                    return;
                }

                var commandContext = _discordClientAccessor.GetCommandContext(userMessage);

                var searchResult = await _commandService
                    .GetExecutableCommandAsync(commandContext, argumentPosition, _serviceProvider);

                if (!searchResult.IsSuccess())
                {
                    await ProcessResultAsync(searchResult.Result, commandContext, channel, argumentPosition, prefix);
                    return;
                }

                var commandMatch = searchResult.CommandMatch;
                var commandInfo = commandMatch.Command;
                var commandName = commandInfo.Name;

                _logger.LogInformation($"Running command: u{userId} {commandName}");

                string? param = null;

                try
                {
                    var paramStart = argumentPosition + commandName.Length;
                    var content = userMessage.Content;
                    var textBigger = content.Length > paramStart;
                    param = textBigger ? content.Substring(paramStart) : null;
                }
                catch (Exception) { }

                var record = new CommandsAnalytics
                {
                    CommandName = commandName,
                    GuildId = guildId,
                    UserId = userId,
                    CreatedOn = _systemClock.UtcNow,
                    CommandParameters = param,
                };

                var commandsAnalyticsRepository = serviceProvider.GetRequiredService<ICommandsAnalyticsRepository>();

                commandsAnalyticsRepository.Add(record);
                await commandsAnalyticsRepository.SaveChangesAsync();

                switch (commandInfo.RunMode)
                {
                    case RunMode.Async:
                        await searchResult.ExecuteAsync(_serviceProvider).ConfigureAwait(false);
                        break;

                    default:
                    case RunMode.Sync:

                        var queueMessage = new CommandMessage(commandMatch, searchResult.Priority)
                        {
                            Context = commandContext,
                            ParseResult = searchResult.ParseResult,
                        };

                        var enqueued = _blockingPriorityQueue.TryEnqueue(queueMessage);

                        if (!enqueued)
                        {
                            await channel.SendMessageAsync(embed: Strings.RejectedCommand.ToEmbedMessage(EMType.Error).Build());
                        }

                        break;
                }
            }
#if DEBUG
            catch (Exception ex)
            {

            }
#endif
        }

        private async Task ProcessResultAsync(
            IResult discordResult,
            ICommandContext context,
            IMessageChannel channel,
            int argumentPosition,
            string prefix)
        {
            if (discordResult == null)
            {
                return;
            }

            Embed embed;

            switch (discordResult.Error)
            {
                case CommandError.UnknownCommand:
                    break;

                case CommandError.MultipleMatches:
                    embed = Strings.MatchedMultipleUsers
                        .ToEmbedMessage(EMType.Error).Build();
                    await channel.SendMessageAsync(embed: embed);
                    break;

                case CommandError.ParseFailed:
                case CommandError.BadArgCount:
                    var searchResult = _commandService.Search(context, argumentPosition);
                    if (searchResult.Commands.Any())
                    {
                        var command = searchResult.Commands.First().Command;
                        await channel.SendMessageAsync(_helperService.GetCommandInfo(command, prefix));
                    }

                    break;

                case CommandError.UnmetPrecondition:
                    var result = PreconditionErrorPayload.Deserialize(discordResult.ErrorReason);

                    var embedBuilder = new EmbedBuilder().WithColor(EMType.Error.Color());

                    if (result.Message != null)
                    {
                        embedBuilder.WithDescription(result.Message);
                    }

                    if (result.ImageUrl == null)
                    {
                        embed = embedBuilder.Build();
                    }
                    else
                    {
                        embed = embedBuilder
                            .WithImageUrl(result.ImageUrl)
                            .Build();
                    }

                    await channel.SendMessageAsync(embed: embed);

                    break;

                default:
                    _logger.LogInformation(discordResult.ErrorReason);
                    break;
            }
        }
    }
}
