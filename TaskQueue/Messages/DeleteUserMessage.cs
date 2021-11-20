namespace Sanakan.TaskQueue.Messages
{
    public class DeleteUserMessage : BaseMessage
    {
        public DeleteUserMessage() : base(Priority.High)
        {
        }

        public ulong DiscordUserId { get; set; }
    }
}
