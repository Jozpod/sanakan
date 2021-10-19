using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Configuration;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class ChaosService
    {
        private readonly DiscordSocketClient _client;
        private List<ulong> _changed;
        private readonly IOptionsMonitor<BotConfiguration> _config;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ILogger _logger;
        private readonly Timer _timer;

        public ChaosService(
            DiscordSocketClient client,
            IRandomNumberGenerator randomNumberGenerator,
            IGuildConfigRepository guildConfigRepository,
            IOptionsMonitor<BotConfiguration> config,
            ILogger<ChaosService> logger)
        {
            _client = client;
            _randomNumberGenerator = randomNumberGenerator;
            _guildConfigRepository = guildConfigRepository;
            _config = config;
            _logger = logger;
            _changed = new List<ulong>();
            _timer = new Timer(_ =>
            {
                try
                {
                    _changed = new List<ulong>();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"in chaos: {ex}", ex);
                    _changed.Clear();
                }
            },
            null,
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(1));

#if !DEBUG
            _client.MessageReceived += HandleMessageAsync;
#endif
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            var msg = message as SocketUserMessage;
            
            if (msg == null)
            {
                return;
            }

            if (msg.Author.IsBot || msg.Author.IsWebhook)
            {
                return;
            }

            var user = msg.Author as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            if (_config.CurrentValue.BlacklistedGuilds.Any(x => x == user.Guild.Id))
            {
                return;
            }

            var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(user.Guild.Id);

            if (gConfig == null)
            {
                return;
            }

            if (!gConfig.ChaosMode)
            {
                return;
            }

            if (_randomNumberGenerator.TakeATry(3))
            {
                var notChangedUsers = user.Guild
                    .Users
                    .Where(x => !x.IsBot
                        && x.Id != user.Id
                        && !_changed.Any(c => c == x.Id))
                    .ToList();

                if (notChangedUsers.Count < 2)
                {
                    return;
                }

                if (_changed.Any(x => x == user.Id))
                {
                    user = _randomNumberGenerator.GetOneRandomFrom(notChangedUsers);
                    notChangedUsers.Remove(user);
                }

                var user2 = _randomNumberGenerator.GetOneRandomFrom(notChangedUsers);

                var user1Nickname = user.Nickname ?? user.Username;
                var user2Nickname = user2.Nickname ?? user2.Username;

                await user.ModifyAsync(x => x.Nickname = user2Nickname);
                _changed.Add(user.Id);

                await user2.ModifyAsync(x => x.Nickname = user1Nickname);
                _changed.Add(user2.Id);
            }
        }
    }
}
