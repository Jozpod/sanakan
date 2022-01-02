namespace Sanakan.TaskQueue.Messages
{
    public class UpdateCardPictureMessage : BaseMessage
    {
        public UpdateCardPictureMessage()
            : base(Priority.Low)
        {
        }

        public ulong CharacterId { get; set; }

        public ulong PictureId { get; set; }
    }
}
