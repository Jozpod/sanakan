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
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly CommandService _commandService;
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<DiscordConfiguration> _config;
        private readonly IHelperService _helperService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CommandHandler(
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            CommandService commandService,
            IOptionsMonitor<DiscordConfiguration> config,
            ILogger<CommandHandler> logger,
            ISystemClock systemClock,
            IServiceProvider serviceProvider,
            IServiceScopeFactory scopeFactory)
        {
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _commandService = commandService;
            _config = config;
            _logger = logger;
            _systemClock = systemClock;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = scopeFactory;
        }

        public async Task InitializeAsync()
        {
            if(_discordSocketClientAccessor.Client == null)
            {
                throw new Exception("Client not connected");
            }

            var client = _discordSocketClientAccessor.Client;

            _helperService.AddPublicModuleInfo(await _commandService
                .AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider));

            _helperService.AddPrivateModuleInfo(
                ("Moderacja", await _commandService.AddModuleAsync<DiscordBot.Modules.ModerationModule>(_serviceProvider)),
                ("Debug", await _commandService.AddModuleAsync<DiscordBot.Modules.DebugModule>(_serviceProvider)));

            client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            if (_discordSocketClientAccessor.Client == null)
            {
                throw new Exception("Client not connected");
            }

            var client = _discordSocketClientAccessor.Client;

            var userMessage = message as SocketUserMessage;
            
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
            var context = new SocketCommandContext(client, userMessage);
            using var serviceScope = _serviceScopeFactory.CreateScope();

            if (context.Guild != null)
            {
                var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

                var gConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(context.Guild.Id);

                if (gConfig?.Prefix != null)
                {
                    prefix = gConfig.Prefix;
                }
            }

            var argPos = 0;

            if (!userMessage.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var isDev = config.AllowedToDebug.Any(x => x == context.User.Id);
            var isOnBlacklist = config.BlacklistedGuilds.Any(x => x == (context.Guild?.Id ?? 0));

            if (isOnBlacklist && !isDev)
            {
                return;
            }

            var searchResult = await _commandService
                .GetExecutableCommandAsync(context, argPos, _serviceProvider);

            if (!searchResult.IsSuccess())
            {
                await ProcessResultAsync(searchResult.Result, context, argPos, prefix);
                return;
            }

            var command = searchResult.Command;


            _logger.LogInformation($"Running command: u{userMessage.Author.Id} {command.Match.Command.Name}");

            string? param = null;

            try
            {
                var paramStart = argPos + searchResult.Command.Match.Command.Name.Length;
                var textBigger = context.Message.Content.Length > paramStart;
                param = textBigger ? context.Message.Content.Substring(paramStart) : null;
            }
            catch (Exception) { }

            var record = new CommandsAnalytics
            {
                CommandName = command.Match.Command.Name,
                GuildId = context.Guild?.Id ?? 0,
                UserId = context.User.Id,
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
                    await context.Channel.SendMessageAsync("", embed: "Odrzucono polecenie!".ToEmbedMessage(EMType.Error).Build());
                    break;
            }
        }

        private async Task ProcessResultAsync(
            IResult discordResult,
            SocketCommandContext context,
            int argPos,
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
                    await context.Channel.SendMessageAsync("", embed: "Dopasowano wielu użytkowników!"
                        .ToEmbedMessage(EMType.Error).Build());
                    break;

                case CommandError.ParseFailed:
                case CommandError.BadArgCount:
                    var searchResult = _commandService.Search(context, argPos);
                    if (searchResult.Commands.Any())
                    {
                        var command = searchResult.Commands.First().Command;
                        await context.Channel.SendMessageAsync(_helperService.GetCommandInfo(command, prefix));
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

                    await context.Channel.SendMessageAsync("", embed: embedBuilder.Build());
                    break;

                default:
                    _logger.LogInformation(discordResult.ErrorReason);
                    break;
            }
        }
    }
}
