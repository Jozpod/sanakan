namespace Sanakan.TaskQueue.Messages
{
    public class ReplaceCharacterIdsInCardMessage : BaseMessage
    {
        public ReplaceCharacterIdsInCardMessage()
            : base(Priority.Low)
        {
        }

        public ulong OldCharacterId { get; set; }

        public ulong NewCharacterId { get; set; }
    }
}
