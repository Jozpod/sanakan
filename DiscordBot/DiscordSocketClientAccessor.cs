using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DiscordBot
{
    public class DiscordSocketClientAccessor : IDiscordSocketClientAccessor
    {
        public DiscordSocketClient? Client { get; set; }
    }
}
