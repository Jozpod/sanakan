using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    public interface IDiscordSocketClientAccessor
    {
        Task LoginAsync(TokenType tokenType, string token, bool validateToken = true);
        Task SetGameAsync(string name, string streamUrl = null, ActivityType type = ActivityType.Playing);
        Task LogoutAsync();
        /// <inheritdoc cref="IDiscordClient"/>
        IDiscordClient Client { get; }
        /// <inheritdoc cref="DiscordSocketClient.Log"/>
        event Func<LogMessage, Task> Log;
        /// <inheritdoc cref="DiscordSocketClient.LeftGuild"/>
        event Func<IGuild, Task> LeftGuild;
        /// <inheritdoc cref="DiscordSocketClient.MessageReceived"/>
        event Func<IMessage, Task> MessageReceived;
        /// <inheritdoc cref="DiscordSocketClient.UserJoined"/>
        event Func<IGuildUser, Task> UserJoined;
        /// <inheritdoc cref="DiscordSocketClient.UserLeft"/>
        event Func<IGuildUser, Task> UserLeft;
        /// <inheritdoc cref="DiscordSocketClient.ReactionAdded"/>
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, IReaction, Task> ReactionAdded;
        /// <inheritdoc cref="DiscordSocketClient.ReactionRemoved"/>
        event Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, IReaction, Task> ReactionRemoved;
        /// <inheritdoc cref="DiscordSocketClient.MessageDeleted"/>
        event Func<Cacheable<IMessage, ulong>, ISocketMessageChannel, Task> MessageDeleted;
        /// <inheritdoc cref="DiscordSocketClient.MessageUpdated"/>
        event Func<Cacheable<IMessage, ulong>, IMessage, ISocketMessageChannel, Task> MessageUpdated;
        /// <inheritdoc cref="DiscordSocketClient.Disconnected"/>
        event Func<Task> LoggedIn;
        /// <inheritdoc cref="DiscordSocketClient.Disconnected"/>
        event Func<Task> LoggedOut;
        /// <inheritdoc cref="DiscordSocketClient.Disconnected"/>
        event Func<Exception, Task> Disconnected;
    }
}
