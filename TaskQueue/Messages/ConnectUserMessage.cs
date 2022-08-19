namespace Sanakan.TaskQueue.Messages
{
    public class ConnectUserMessage : BaseMessage
    {
        public ConnectUserMessage()
            : base(Priority.High)
        {
        }

        public ulong DiscordUserId { get; set; }

        public ulong ShindenUserId { get; set; }
    }
}
