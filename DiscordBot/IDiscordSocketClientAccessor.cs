using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DiscordBot
{
    public interface IDiscordSocketClientAccessor
    {
        public DiscordSocketClient? Client { get; set; }
    }
}
