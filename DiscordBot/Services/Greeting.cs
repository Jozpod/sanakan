using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Services.Executor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using Sanakan.Web.Configuration;

namespace Sanakan.Services
{
    public class Greeting
    {
        private readonly DiscordSocketClient _client;
        private readonly IExecutor _executor;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;
        private readonly ICacheManager _cacheManager;

        public Greeting(
            DiscordSocketClient client,
            ILogger<Greeting> logger,
            IOptionsMonitor<SanakanConfiguration> config,
            IExecutor exe,
            ICacheManager _cacheManager)
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
            var gConfig = await db.GetGuildConfigOrCreateAsync(guild.Id);
            db.Guilds.Remove(gConfig);

            var stats = db.TimeStatuses
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Guild == guild.Id)
                .ToList();
            db.TimeStatuses.RemoveRange(stats);

            await db.SaveChangesAsync();

            var mute = db.Penalties
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Guild == guild.Id)
                .ToList();

            db.Penalties.RemoveRange(mute);

            await db.SaveChangesAsync();
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            if (user.IsBot || user.IsWebhook)
            {
                return;
            }

            if (_config.BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            var guildConfig = await db.GetCachedGuildFullConfigAsync(user.Guild.Id);

            if (guildConfig?.WelcomeMessage == null)
            {
                return;
            }

            if (guildConfig.WelcomeMessage == "off")
            {
                return;
            }

            await SendMessageAsync(ReplaceTags(user, config.WelcomeMessage), user.Guild.GetTextChannel(config.GreetingChannel));

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

            if (!config.BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                var guildConfig = await db.GetCachedGuildFullConfigAsync(user.Guild.Id);
                if (guildConfig?.GoodbyeMessage == null)
                {
                    return;
                }

                if (guildConfig.GoodbyeMessage == "off")
                {
                    return;
                }

                await SendMessageAsync(ReplaceTags(user, config.GoodbyeMessage), user.Guild.GetTextChannel(config.GreetingChannel));
            }

            var thisUser = _client.Guilds.FirstOrDefault(x => x.Id == user.Id);
            if (thisUser != null)
            {
                return;
            }

            var moveTask = new Task<Task>(async () =>
            {
                var duser = await db.GetUserOrCreateAsync(user.Id);
                var fakeu = await db.GetUserOrCreateAsync(1);

                foreach (var card in duser.GameDeck.Cards)
                {
                    card.InCage = false;
                    card.TagList.Clear();
                    card.LastIdOwner = user.Id;
                    card.GameDeckId = fakeu.GameDeck.Id;
                }

                db.Users.Remove(duser);

                await db.SaveChangesAsync();

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