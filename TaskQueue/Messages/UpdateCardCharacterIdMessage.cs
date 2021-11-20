namespace Sanakan.TaskQueue.Messages
{
    public class UpdateCardCharacterIdMessage : BaseMessage
    {
        public UpdateCardCharacterIdMessage() : base(Priority.Low) { }

        public ulong CharacterId { get; set; }
    }
}
