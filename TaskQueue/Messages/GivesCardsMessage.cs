namespace Sanakan.TaskQueue.Messages
{
    public class GivesCardsMessage : BaseMessage
    {
        public GivesCardsMessage() : base(Priority.Low) { }

        public ulong CharacterId { get; set; }
    }
}
