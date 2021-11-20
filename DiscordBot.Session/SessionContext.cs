using Discord;

namespace Sanakan.DiscordBot.Session
{
    public class SessionContext
    {
        public IMessageChannel Channel { get; set; }
        public IUser User { get; set; }
        public IUserMessage Message { get; set; }
        public IDiscordClient Client { get; set; }
        public IReaction? AddReaction { get; set; }
        public IReaction? RemoveReaction { get; set; }
    }
}
