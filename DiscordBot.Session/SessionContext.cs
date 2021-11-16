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
        public IMessageChannel Channel { get; set; }
        public IUser User { get; set; }
        public IUserMessage Message { get; set; }
        public IDiscordClient Client { get; set; }
        public IReaction? AddReaction { get; set; }
        public IReaction? RemoveReaction { get; set; }
    }
}
