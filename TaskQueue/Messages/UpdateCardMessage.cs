namespace Sanakan.TaskQueue.Messages
{
    public class UpdateCardMessage : BaseMessage
    {
        public UpdateCardMessage()
            : base(Priority.Low)
        {
        }

        public ulong CharacterId { get; set; }

        public string? ImageUrl { get; set; }

        public string CharacterName { get; set; } = null;

        public string CardSeriesTitle { get; set; } = null;
    }
}
