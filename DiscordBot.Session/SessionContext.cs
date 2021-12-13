using Discord;

namespace Sanakan.DiscordBot.Session
{
    public class SessionContext
    {
        public IMessageChannel Channel { get; set; } = null;

        public ulong UserId { get; set; }

        public IUserMessage Message { get; set; } = null!;

        public IDiscordClient Client { get; set; } = null;

        public IReaction? AddReaction { get; set; }

        public IReaction? RemoveReaction { get; set; }
    }
}
