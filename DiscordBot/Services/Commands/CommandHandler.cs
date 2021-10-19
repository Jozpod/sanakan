using DAL.Repositories.Abstractions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Services.PocketWaifu;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Configuration;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Services.Commands
{
    public class CommandHandler
    {
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly CommandService _commandService;
        private readonly IExecutor _executor;
        private readonly ILogger _logger;
        private readonly IAllRepository _repository;
        private readonly ICommandsAnalyticsRepository _commandsAnalyticsRepository;
        
        private readonly ISystemClock _systemClock;
        private readonly IOptionsMonitor<BotConfiguration> _config;
        private readonly HelperService _helper;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public CommandHandler(
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<BotConfiguration> config,
            ILogger<CommandHandler> logger,
            IExecutor executor,
            IAllRepository repository,
            CommandService commandService,
            ISystemClock systemClock,
            IServiceProvider serviceProvider)
        {
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _config = config;
            _logger = logger;
            _executor = executor;
            _repository = repository;
            _commandService = commandService;
            _systemClock = systemClock;
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            if(_discordSocketClientAccessor.Client == null)
            {
                throw new Exception("Client not connected");
            }
            var client = _discordSocketClientAccessor.Client;

            _commandService.AddTypeReader<SlotMachineSetting>(new TypeReaders.SlotMachineSettingTypeReader());
            _commandService.AddTypeReader<WishlistObjectType>(new TypeReaders.WishlistObjectTypeReader());
            _commandService.AddTypeReader<CardExpedition>(new TypeReaders.ExpeditionTypeReader());
            _commandService.AddTypeReader<ProfileType>(new TypeReaders.ProfileTypeReader());
            _commandService.AddTypeReader<ConfigType>(new TypeReaders.ConfigTypeReader());
            _commandService.AddTypeReader<CoinSide>(new TypeReaders.CoinSideTypeReader());
            _commandService.AddTypeReader<HaremType>(new TypeReaders.HaremTypeReader());
            _commandService.AddTypeReader<TopType>(new TypeReaders.TopTypeReader());
            _commandService.AddTypeReader<bool>(new TypeReaders.BoolTypeReader());

            _helper.PublicModulesInfo = await _commandService
                .AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

            _helper.PrivateModulesInfo.Add("Moderacja", await _commandService.AddModuleAsync<Modules.ModerationModule>(_serviceProvider));
            _helper.PrivateModulesInfo.Add("Debug", await _commandService.AddModuleAsync<Modules.DebugModule>(_serviceProvider));

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

            if (context.Guild != null)
            {
                var gConfig = await _repository.GetCachedGuildFullConfigAsync(context.Guild.Id);
                if (gConfig?.Prefix != null) prefix = gConfig.Prefix;
            }

            var argPos = 0;
            if (!userMessage.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var isDev = config.Dev.Any(x => x == context.User.Id);
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
                CmdName = command.Match.Command.Name,
                GuildId = context.Guild?.Id ?? 0,
                UserId = context.User.Id,
                Date = _systemClock.UtcNow,
                CmdParams = param,
            };

            _commandsAnalyticsRepository.Add(record);
            await _commandsAnalyticsRepository.SaveChangesAsync();

            switch (command.Match.Command.RunMode)
            {
                case RunMode.Async:
                    await command.ExecuteAsync(_serviceProvider);
                    break;

                default:
                case RunMode.Sync:
                    if (!await _executor.TryAdd(command, TimeSpan.FromSeconds(1)))
                    {
                        await context.Channel.SendMessageAsync("", embed: "Odrzucono polecenie!".ToEmbedMessage(EMType.Error).Build());
                    }
                    break;
            }
        }

        private async Task ProcessResultAsync(
            IResult result,
            SocketCommandContext context,
            int argPos,
            string prefix)
        {
            if (result == null)
            {
                return;
            }

            switch (result.Error)
            {
                case CommandError.UnknownCommand:
                    break;

                case CommandError.MultipleMatches:
                    await context.Channel.SendMessageAsync("", embed: "Dopasowano wielu użytkowników!".ToEmbedMessage(EMType.Error).Build());
                    break;

                case CommandError.ParseFailed:
                case CommandError.BadArgCount:
                    var cmd = _commandService.Search(context, argPos);
                    if (cmd.Commands.Count > 0)
                    {
                        await context.Channel.SendMessageAsync(_helper.GetCommandInfo(cmd.Commands.First().Command, prefix));
                    }
                    break;

                case CommandError.UnmetPrecondition:
                    if (result.ErrorReason.StartsWith("|IMAGE|"))
                    {
                        var emb = new EmbedBuilder().WithColor(EMType.Error.Color());
                        var splited = result.ErrorReason.Split("|");
                        
                        if (splited.Length > 3)
                        {
                            emb.WithDescription(splited[3]).WithImageUrl(splited[2]);
                        }
                        else emb.WithImageUrl(result.ErrorReason.Remove(0, 7));

                        await context.Channel.SendMessageAsync("", embed: emb.Build());
                    }
                    else await context.Channel.SendMessageAsync("", embed: result.ErrorReason.ToEmbedMessage(EMType.Error).Build());
                    break;

                default:
                    _logger.LogInformation(result.ErrorReason);
                    break;
            }
        }
    }
}
