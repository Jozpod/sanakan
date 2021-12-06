using Discord;
using Discord.Commands;

namespace Sanakan.TaskQueue.Messages
{
    public class CommandMessage : BaseMessage
    {
        public CommandMessage(Priority priority) : base(priority) {}

        public CommandMatch Match { get; set; }
        public ParseResult ParseResult { get; set; }
        public ICommandContext Context { get; set; }
    }
}
