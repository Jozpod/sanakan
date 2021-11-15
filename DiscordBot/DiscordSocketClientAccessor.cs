using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    internal class DiscordSocketClientAccessor : IDiscordSocketClientAccessor
    {
        private readonly DiscordSocketClient _client;
        public DiscordSocketClientAccessor(DiscordSocketClient client)
        {
            _client = client;
            client.MessageReceived += MessageReceived;
            client.ReactionAdded += ReactionAdded;
            client.ReactionRemoved += ReactionRemoved;
            client.UserJoined += UserJoined;
            client.Log += Log;
            client.LoggedIn += LoggedIn;
            client.LoggedOut += LoggedOut;
            client.Disconnected += Disconnected;
            client.UserLeft += UserLeft;
            client.LeftGuild += LeftGuild;
            client.MessageUpdated += MessageUpdated;
        }

        public IDiscordClient? Client => _client;
        public Task LogoutAsync() => _client.LogoutAsync();

        public Task SetGameAsync(string name,
            string streamUrl = null,
            ActivityType type = ActivityType.Playing) => _client.SetGameAsync(name, streamUrl, type);

        public Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
            => _client.LoginAsync(tokenType, token, validateToken);

        public event Func<IGuildUser, Task> UserJoined;
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, IReaction, Task> ReactionAdded;
        public event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, IReaction, Task> ReactionRemoved;
        public event Func<IMessage, Task> MessageReceived;
        public event Func<LogMessage, Task> Log;
        public event Func<IGuild, Task> LeftGuild;
        public event Func<IGuildUser, Task> UserLeft;
        public event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
        public event Func<Cacheable<IMessage, ulong>, IMessage, ISocketMessageChannel, Task> MessageUpdated;
        public event Func<Exception, Task> Disconnected;
        public event Func<Task> LoggedIn;
        public event Func<Task> LoggedOut;
    }
}
