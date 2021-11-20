using Discord;

namespace Sanakan.TaskQueue.Messages
{
    public class AddExperienceMessage : BaseMessage
    {
        public AddExperienceMessage() : base(Priority.High) {}

        public ulong DiscordUserId { get; set; }
        public ulong ShindenUserId { get; set; }
        public long Experience { get; set; }
        public ulong MessageCount { get; set; }
        public ulong CharacterCount { get; set; }
        public ulong CommandCount { get; set; }
        public bool CalculateExperience { get; set; }
        public ulong GuildId { get; set; }
        public IGuildUser User { get; set; }
        public IMessageChannel Channel { get; set; }
    }
}
