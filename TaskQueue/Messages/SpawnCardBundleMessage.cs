using Discord;

namespace Sanakan.TaskQueue.Messages
{
    public class SpawnCardBundleMessage : BaseMessage
    {
        public SpawnCardBundleMessage()
            : base(Priority.Low)
        {
        }

        public ulong DiscordUserId { get; set; }

        public ulong? GuildId { get; set; }

        public IMessageChannel MessageChannel { get; set; } = null;

        public string Mention { get; set; } = null;
    }
}
