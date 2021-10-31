using Discord;
using Discord.WebSocket;
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
        public ulong WinnerUserId { get; set; }
        public uint CardCount { get; set; }
        public IUser WinnerUser { get; set; }
        public ISocketMessageChannel Channel { get; set; }
        public ulong InvokingUserId { get; set; }
    }
}
