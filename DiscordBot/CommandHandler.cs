using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Services.PocketWaifu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Extensions;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Models;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    internal class CommandHandler : ICommandHandler
    {
        private readonly IDiscordClientAccessor _discordSocketClientAccessor;
        private readonly IHelperService _helperService;
        private readonly ICommandService _commandService;
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CommandHandler(
            IDiscordClientAccessor discordSocketClientAccessor,
            IHelperService helperService,
            ICommandService commandService,
            IOptionsMonitor<DiscordConfiguration> config,
            ILogger<CommandHandler> logger,
            ISystemClock systemClock,
            IServiceProvider serviceProvider,
            IServiceScopeFactory scopeFactory)
        {
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _helperService = helperService;
            _commandService = commandService;
            _config = config;
            _logger = logger;
            _systemClock = systemClock;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = scopeFactory;
        }

        public async Task InitializeAsync()
        {
            var client = _discordSocketClientAccessor.Client;

            _helperService.AddPublicModuleInfo(await _commandService
                .AddModulesAsync(typeof(CommandHandler).Assembly, _serviceProvider));

            _helperService.AddPrivateModuleInfo(
                ("Moderacja", await _commandService.AddModuleAsync<DiscordBot.Modules.ModerationModule>(_serviceProvider)),
                ("Debug", await _commandService.AddModuleAsync<DiscordBot.Modules.DebugModule>(_serviceProvider)));

            _discordSocketClientAccessor.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(IMessage message)
        {
            var userMessage = message as IUserMessage;
            
            if (userMessage == null)
            {
                return;
            }

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
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
            var client = _discordSocketClientAccessor.Client;
            var channel = (IMessageChannel)await client.GetChannelAsync(userMessage.Channel.Id);

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();
            var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (gConfig?.Prefix != null)
            {
                prefix = gConfig.Prefix;
            }

            var argumentPosition = 0;

            if (!userMessage.HasStringPrefix(prefix, ref argumentPosition, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var isDev = config.AllowedToDebug.Any(x => x == guildUser.Id);
            var isOnBlacklist = config.BlacklistedGuilds.Any(x => x == guild.Id);

            if (isOnBlacklist && !isDev)
            {
                return;
            }
            
            var context = _discordSocketClientAccessor.GetCommandContext(userMessage);

            var searchResult = await _commandService
                .GetExecutableCommandAsync(context, argumentPosition, _serviceProvider);

            if (!searchResult.IsSuccess())
            {
                await ProcessResultAsync(searchResult.Result, context, channel, argumentPosition, prefix);
                return;
            }

            var command = searchResult.Command;


            _logger.LogInformation($"Running command: u{guildUser.Id} {command.Match.Command.Name}");

            string? param = null;

            try
            {
                var paramStart = argumentPosition + searchResult.Command.Match.Command.Name.Length;
                var content = userMessage.Content;
                var textBigger = content.Length > paramStart;
                param = textBigger ? content.Substring(paramStart) : null;
            }
            catch (Exception) { }

            var record = new CommandsAnalytics
            {
                CommandName = command.Match.Command.Name,
                GuildId = guild.Id,
                UserId = guildUser.Id,
                Date = _systemClock.UtcNow,
                CommandParameters = param,
            };

            var commandsAnalyticsRepository = serviceScope.ServiceProvider.GetRequiredService<ICommandsAnalyticsRepository>();

            commandsAnalyticsRepository.Add(record);
            await commandsAnalyticsRepository.SaveChangesAsync();

            switch (command.Match.Command.RunMode)
            {
                case RunMode.Async:
                    await command.ExecuteAsync(_serviceProvider);
                    break;

                default:
                case RunMode.Sync:
                    await channel.SendMessageAsync("", embed: "Odrzucono polecenie!".ToEmbedMessage(EMType.Error).Build());
                    break;
            }
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

            switch (discordResult.Error)
            {
                case CommandError.UnknownCommand:
                    break;

                case CommandError.MultipleMatches:
                    await channel.SendMessageAsync("", embed: "Dopasowano wielu użytkowników!"
                        .ToEmbedMessage(EMType.Error).Build());
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

                    if (result.ImageUrl != null)
                    {
                        embedBuilder.WithImageUrl(result.ImageUrl);
                    }

                    await channel.SendMessageAsync("", embed: embedBuilder.Build());
                    break;

                default:
                    _logger.LogInformation(discordResult.ErrorReason);
                    break;
            }
        }
    }
}
