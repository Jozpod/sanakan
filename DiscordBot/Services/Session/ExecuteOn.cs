using System;

namespace DiscordBot.Services.Session
{
    [Flags]
    public enum ExecuteOn
    {
        None = 0,
        Message = 1 << 0,
        ReactionAdded = 1 << 1,
        ReactionRemoved = 1 << 2,

        AllReactions = ReactionAdded | ReactionRemoved,
        AllEvents = Message | AllReactions,
    }
}
