namespace Sanakan.TaskQueue.Messages
{
    public class AddExperienceMessage : BaseMessage
    {
        public AddExperienceMessage() : base(Priority.High) {}

        public ulong DiscordUserId { get; set; }
        public ulong ShindenUserId { get; set; }
    }
}
