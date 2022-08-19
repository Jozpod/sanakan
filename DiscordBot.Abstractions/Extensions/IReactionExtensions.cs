using Discord;
using Discord.WebSocket;

namespace Sanakan.DiscordBot.Abstractions.Extensions
{
    public static class IReactionExtensions
    {
        public static ulong? GetUserId(this IReaction reaction)
        {
            return (reaction as SocketReaction)?.UserId;
        }
    }
}
