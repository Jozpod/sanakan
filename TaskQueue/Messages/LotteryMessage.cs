using Discord;

namespace Sanakan.TaskQueue.Messages
{
    public class LotteryMessage : BaseMessage
    {
        public LotteryMessage()
            : base(Priority.High)
        {
        }

        public ulong DiscordUserId { get; set; }

        public IUserMessage UserMessage { get; set; } = null;

        public ulong WinnerUserId { get; set; }

        public uint CardCount { get; set; }

        public IUser WinnerUser { get; set; } = null;

        public IMessageChannel Channel { get; set; } = null;

        public ulong InvokingUserId { get; set; }
    }
}
