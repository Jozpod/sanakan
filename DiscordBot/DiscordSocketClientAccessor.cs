using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    [ExcludeFromCodeCoverage]
    internal class DiscordSocketClientAccessor : IDiscordClientAccessor
    {
        private readonly DiscordSocketClient _client;
        private readonly object _syncRoot = new();

        public DiscordSocketClientAccessor(DiscordSocketClient client)
        {
            _client = client;
        }

        public event Func<Task> Ready
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.Ready += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.Ready -= value;
                }
            }
        }

        public event Func<IGuildUser, Task> UserJoined
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.UserJoined += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.UserJoined -= value;
                }
            }
        }

        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, IReaction, Task> ReactionAdded
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.ReactionAdded += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.ReactionAdded -= value;
                }
            }
        }

        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, IReaction, Task> ReactionRemoved
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.ReactionRemoved += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.ReactionRemoved -= value;
                }
            }
        }

        public event Func<IMessage, Task> MessageReceived
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.MessageReceived += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.MessageReceived -= value;
                }
            }
        }

        public event Func<LogMessage, Task> Log
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.Log += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.Log -= value;
                }
            }
        }

        public event Func<IGuild, Task> LeftGuild
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.LeftGuild += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.LeftGuild -= value;
                }
            }
        }

        public event Func<IGuildUser, Task> UserLeft
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.UserLeft += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.UserLeft -= value;
                }
            }
        }

        public event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.MessageDeleted += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.MessageDeleted -= value;
                }
            }
        }

        public event Func<Cacheable<IMessage, ulong>, IMessage, ISocketMessageChannel, Task> MessageUpdated
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.MessageUpdated += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.MessageUpdated -= value;
                }
            }
        }

        public event Func<Exception, Task> Disconnected
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.Disconnected += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.Disconnected -= value;
                }
            }
        }

        public event Func<Task> LoggedIn
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.LoggedIn += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.LoggedIn -= value;
                }
            }
        }

        public event Func<Task> LoggedOut
        {
            add
            {
                lock (_syncRoot)
                {
                    _client.LoggedOut += value;
                }
            }

            remove
            {
                lock (_syncRoot)
                {
                    _client.LoggedOut -= value;
                }
            }
        }

        public IDiscordClient Client => _client;

        public int Latency => _client.Latency;

        public Task LogoutAsync() => _client.LogoutAsync();

        public Task SetGameAsync(
            string name,
            string? streamUrl = null,
            ActivityType type = ActivityType.Playing) => _client.SetGameAsync(name, streamUrl, type);

        public Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
            => _client.LoginAsync(tokenType, token, validateToken);

        public ICommandContext GetCommandContext(IUserMessage message) => new CommandContext(_client, message);
    }
}
