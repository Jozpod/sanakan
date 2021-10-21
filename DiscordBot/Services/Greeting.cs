using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Services.Executor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Configuration;
using Sanakan.Extensions;
using Sanakan.Services.Executor;

namespace Sanakan.Services
{
    public class Greeting
    {
        private readonly DiscordSocketClient _client;
        private readonly IExecutor _executor;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<BotConfiguration> _config;
        private readonly ICacheManager _cacheManager;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ITimeStatusRepository _timeStatusRepository;
        private readonly IPenaltyInfoRepository _penaltyInfoRepository;
        private readonly IUserRepository _userRepository;
        public Greeting(
            DiscordSocketClient client,
            ILogger<Greeting> logger,
            IOptionsMonitor<BotConfiguration> config,
            IExecutor exe,
            ICacheManager cacheManager)
        {
            _client = client;
            _logger = logger;
            _config = config;
            _executor = exe;
            _cacheManager = cacheManager;

#if !DEBUG
            _client.LeftGuild += BotLeftGuildAsync;
            _client.UserJoined += UserJoinedAsync;
            _client.UserLeft += UserLeftAsync;
#endif
        }

        private async Task BotLeftGuildAsync(SocketGuild guild)
        {
            var gConfig = await _guildConfigRepository.GetGuildConfigOrCreateAsync(guild.Id);
            _guildConfigRepository.Remove(gConfig);
           
            var stats = await _timeStatusRepository.GetByGuildIdAsync(guild.Id);
            _timeStatusRepository.RemoveRange(stats);

            await _timeStatusRepository.SaveChangesAsync();

            var mutes = await _penaltyInfoRepository.GetByGuildIdAsync(guild.Id);
            _penaltyInfoRepository.RemoveRange(mutes);

            await _penaltyInfoRepository.SaveChangesAsync();
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            var guildId = user.Guild.Id;

            if (_config.CurrentValue
                .BlacklistedGuilds.Any(x => x == guildId))
            {
                return;
            }

            var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);

            if (guildConfig?.WelcomeMessage == null)
            {
                return;
            }

            if (guildConfig.WelcomeMessage == "off")
            {
                return;
            }

            var content = ReplaceTags(user, guildConfig.WelcomeMessage);
            var textChannel = user.Guild.GetTextChannel(guildConfig.GreetingChannel);
            await SendMessageAsync(content, textChannel);

            if (guildConfig?.WelcomeMessagePW == null)
            {
                return;
            }

            if (guildConfig.WelcomeMessagePW == "off")
            {
                return;
            }

            try
            {
                var pw = await user.GetOrCreateDMChannelAsync();
                await pw.SendMessageAsync(ReplaceTags(user, guildConfig.WelcomeMessagePW));
                await pw.CloseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Greeting: {ex}", ex);
            }
        }

        private async Task UserLeftAsync(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            var config = _config.CurrentValue;
            var guildId = user.Guild.Id;

            if (!config.BlacklistedGuilds.Any(x => x == guildId))
            {
                var guildConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guildId);
                if (guildConfig?.GoodbyeMessage == null)
                {
                    return;
                }

                if (guildConfig.GoodbyeMessage == "off")
                {
                    return;
                }

                var content = ReplaceTags(user, guildConfig.GoodbyeMessage);
                var textChannel = user.Guild.GetTextChannel(guildConfig.GreetingChannel);
                await SendMessageAsync(content, textChannel);
            }

            var thisUser = _client.Guilds.FirstOrDefault(x => x.Id == user.Id);
            if (thisUser != null)
            {
                return;
            }

            var moveTask = new Task<Task>(async () =>
            {
                var duser = await _userRepository.GetUserOrCreateAsync(user.Id);
                var fakeu = await _userRepository.GetUserOrCreateAsync(1);

                foreach (var card in duser.GameDeck.Cards)
                {
                    card.InCage = false;
                    card.TagList.Clear();
                    card.LastIdOwner = user.Id;
                    card.GameDeckId = fakeu.GameDeck.Id;
                }

                _userRepository.Remove(duser);

                await _userRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { "users" });
            });

            await _executor.TryAdd(new Executable("delete user", moveTask, Priority.High), TimeSpan.FromSeconds(1));
        }

        private async Task SendMessageAsync(string message, ITextChannel channel)
        {
            if (channel != null) await channel.SendMessageAsync(message);
        }

        private string ReplaceTags(SocketGuildUser user, string message)
            => message.Replace("^nick", user.Nickname ?? user.Username).Replace("^mention", user.Mention);
    }
}