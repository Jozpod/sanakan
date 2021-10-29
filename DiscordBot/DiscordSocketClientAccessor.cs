using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    internal class DiscordSocketClientAccessor : IDiscordSocketClientAccessor
    {
        public DiscordSocketClient? Client { get; set; }

        public event Func<Task> Initialized;
    }
}
