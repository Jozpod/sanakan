using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
