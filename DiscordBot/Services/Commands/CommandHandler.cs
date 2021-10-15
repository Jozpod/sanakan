using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DiscordBot.Services;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using Sanakan.Services.PocketWaifu;
using Shinden.Logger;
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
        private object _config;
        private Helper _helper;
        private Timer _timer;

        public CommandHandler(
            DiscordSocketClient client,
            IOptions<object> config,
            ILogger<CommandHandler> logger,
            IExecutor executor)
        {
            _client = client;
            _config = config.Value;
            _logger = logger;
            _executor = executor;
            _cmd = new CommandService();
        }

        public async Task InitializeAsync(IServiceProvider provider, Helper helper)
        {
            _helper = helper;
            _provider = provider;

            _cmd.AddTypeReader<SlotMachineSetting>(new TypeReaders.SlotMachineSettingTypeReader());
            _cmd.AddTypeReader<WishlistObjectType>(new TypeReaders.WishlistObjectTypeReader());
            _cmd.AddTypeReader<CardExpedition>(new TypeReaders.ExpeditionTypeReader());
            _cmd.AddTypeReader<ProfileType>(new TypeReaders.ProfileTypeReader());
            _cmd.AddTypeReader<ConfigType>(new TypeReaders.ConfigTypeReader());
            _cmd.AddTypeReader<CoinSide>(new TypeReaders.CoinSideTypeReader());
            _cmd.AddTypeReader<HaremType>(new TypeReaders.HaremTypeReader());
            _cmd.AddTypeReader<TopType>(new TypeReaders.TopTypeReader());
            _cmd.AddTypeReader<bool>(new TypeReaders.BoolTypeReader());

            _helper.PublicModulesInfo = await _cmd.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            _helper.PrivateModulesInfo.Add("Moderacja", await _cmd.AddModuleAsync<Modules.Moderation>(_provider));
            _helper.PrivateModulesInfo.Add("Debug", await _cmd.AddModuleAsync<Modules.Debug>(_provider));

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

            var conf = _config.Get();
            string prefix = conf.Prefix;
            var context = new SocketCommandContext(_client, msg);

            if (context.Guild != null)
            {
                var gConfig = await db.GetCachedGuildFullConfigAsync(context.Guild.Id);
                if (gConfig?.Prefix != null) prefix = gConfig.Prefix;
            }

            int argPos = 0;
            if (!userMessage.HasStringPrefix(prefix, ref argPos, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var isDev = conf.Dev.Any(x => x == context.User.Id);
            var isOnBlacklist = conf.BlacklistedGuilds.Any(x => x == (context.Guild?.Id ?? 0));

            if (isOnBlacklist && !isDev)
            {
                return;
            }

            var res = await _cmd.GetExecutableCommandAsync(context, argPos, _provider);

            if (!res.IsSuccess())
            {
                else await ProcessResultAsync(res.Result, context, argPos, prefix);
                return;
            }
     
            _logger.LogInformation($"Run cmd: u{msg.Author.Id} {res.Command.Match.Command.Name}");

            string param = null;
            try
            {
                var paramStart = argPos + res.Command.Match.Command.Name.Length;
                var textBigger = context.Message.Content.Length > paramStart;
                param = textBigger ? context.Message.Content.Substring(paramStart) : null;
            }
            catch (Exception) { }

            dbc.CommandsData.Add(new CommandsAnalytics()
            {
                CmdName = res.Command.Match.Command.Name,
                GuildId = context.Guild?.Id ?? 0,
                UserId = context.User.Id,
                Date = DateTime.Now,
                CmdParams = param,
            });

            await dbc.SaveChangesAsync();

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
            if (result == null) return;

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
                        if (splited.Length > 3) emb.WithDescription(splited[3]).WithImageUrl(splited[2]);
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
