using System;

namespace Sanakan.DiscordBot.Session.Abstractions
{
    [Flags]
    public enum SessionExecuteCondition : byte
    {
        None = 0,
        Message = 1 << 0,
        ReactionAdded = 1 << 1,
        ReactionRemoved = 1 << 2,

        AllReactions = ReactionAdded | ReactionRemoved,
        AllEvents = Message | AllReactions,
    }
}
