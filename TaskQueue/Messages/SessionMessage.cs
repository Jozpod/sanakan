using Sanakan.DiscordBot.Session.Abstractions;

namespace Sanakan.TaskQueue.Messages
{
    public class SessionMessage : BaseMessage
    {
        public SessionMessage() : base(Priority.Medium) { }

        public IInteractionSession Session { get; set; } = null;

        public SessionContext Context { get; set; } = null;
    }
}
