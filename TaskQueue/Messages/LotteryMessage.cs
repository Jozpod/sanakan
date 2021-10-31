using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class LotteryMessage : BaseMessage
    {
        public LotteryMessage() : base(Priority.High) { }

        public ulong DiscordUserId { get; set; }
        public IUserMessage UserMessage { get; set; }
    }
}
