namespace Sanakan.TaskQueue.Messages
{
    public class TransferTCMessage : BaseMessage
    {
        public TransferTCMessage() : base(Priority.Low) { }

        public ulong Amount { get; set; }
        public ulong DiscordUserId { get; set; }
        public ulong ShindenUserId { get; set; }
    }
}
