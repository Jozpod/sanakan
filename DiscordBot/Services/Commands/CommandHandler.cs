using DAL.Repositories.Abstractions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using Sanakan.Web.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Services.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _provider;
        private readonly CommandService _cmd;
        private readonly IExecutor _executor;
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private SanakanConfiguration _config;
        private Helper _helper;
        private Timer _timer;

        public CommandHandler(
            DiscordSocketClient client,
            IOptions<SanakanConfiguration> config,
            ILogger<CommandHandler> logger,
            IExecutor executor,
            IRepository repository)
        {
            _client = client;
            _config = config.Value;
            _logger = logger;
            _executor = executor;
            _repository = repository;
            _cmd = new CommandService();
        }

        public async Task InitializeAsync(IServiceProvider provider, Helper helper)
        {
            _helper = helper;
            _provider = provider;

            //_cmd.AddTypeReader<SlotMachineSetting>(new TypeReaders.SlotMachineSettingTypeReader());
            //_cmd.AddTypeReader<WishlistObjectType>(new TypeReaders.WishlistObjectTypeReader());
            //_cmd.AddTypeReader<CardExpedition>(new TypeReaders.ExpeditionTypeReader());
            //_cmd.AddTypeReader<ProfileType>(new TypeReaders.ProfileTypeReader());
            //_cmd.AddTypeReader<ConfigType>(new TypeReaders.ConfigTypeReader());
            //_cmd.AddTypeReader<CoinSide>(new TypeReaders.CoinSideTypeReader());
            //_cmd.AddTypeReader<HaremType>(new TypeReaders.HaremTypeReader());
            //_cmd.AddTypeReader<TopType>(new TypeReaders.TopTypeReader());
            //_cmd.AddTypeReader<bool>(new TypeReaders.BoolTypeReader());

            _helper.PublicModulesInfo = await _cmd.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            _helper.PrivateModulesInfo.Add("Moderacja", await _cmd.AddModuleAsync<Modules.ModerationModule>(_provider));
            _helper.PrivateModulesInfo.Add("Debug", await _cmd.AddModuleAsync<Modules.DebugModule>(_provider));

            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage message)
        {
            var userMessage = message as SocketUserMessage;
            
            if (userMessage == null)
            {
                return;
            }

            if (userMessage.Author.IsBot || userMessage.Author.IsWebhook)
            {
                return;
            }

            string prefix = _config.Prefix;
            var context = new SocketCommandContext(_client, userMessage);

            if (context.Guild != null)
            {
                var gConfig = await _repository.GetCachedGuildFullConfigAsync(context.Guild.Id);
                if (gConfig?.Prefix != null) prefix = gConfig.Prefix;
            }

            int argPos = 0;
            if (!userMessage.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var isDev = _config.Dev.Any(x => x == context.User.Id);
            var isOnBlacklist = _config.BlacklistedGuilds.Any(x => x == (context.Guild?.Id ?? 0));

            if (isOnBlacklist && !isDev)
            {
                return;
            }

            var res = await _cmd.GetExecutableCommandAsync(context, argPos, _provider);

            if (!res.IsSuccess())
            {
                await ProcessResultAsync(res.Result, context, argPos, prefix);
                return;
            }
     
            _logger.LogInformation($"Run cmd: u{userMessage.Author.Id} {res.Command.Match.Command.Name}");

            string param = null;
            try
            {
                var paramStart = argPos + res.Command.Match.Command.Name.Length;
                var textBigger = context.Message.Content.Length > paramStart;
                param = textBigger ? context.Message.Content.Substring(paramStart) : null;
            }
            catch (Exception) { }

            var record = new CommandsAnalytics()
            {
                CmdName = res.Command.Match.Command.Name,
                GuildId = context.Guild?.Id ?? 0,
                UserId = context.User.Id,
                Date = _systemClock.UtcNow,
                CmdParams = param,
            };

            _repository.CommandsData.Add(record);

            await _repository.SaveChangesAsync();

            switch (res.Command.Match.Command.RunMode)
            {
                case RunMode.Async:
                    await res.Command.ExecuteAsync(_provider);
                    break;

                default:
                case RunMode.Sync:
                    if (!await _executor.TryAdd(res.Command, TimeSpan.FromSeconds(1)))
                            await context.Channel.SendMessageAsync("", embed: "Odrzucono polecenie!".ToEmbedMessage(EMType.Error).Build());
                    break;
            }
        }

        private async Task ProcessResultAsync(IResult result, SocketCommandContext context, int argPos, string prefix)
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
                    var cmd = _cmd.Search(context, argPos);
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
