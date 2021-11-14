using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public class SessionContext
    {
        public ISocketMessageChannel Channel { get; set; }
        public SocketUser User { get; set; }
        public SocketUserMessage Message { get; set; }
        public IDiscordClient Client { get; set; }
        public SocketReaction? AddReaction { get; set; }
        public SocketReaction? RemoveReaction { get; set; }
    }
}
