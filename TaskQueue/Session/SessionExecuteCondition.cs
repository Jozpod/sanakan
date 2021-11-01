using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    [Flags]
    public enum SessionExecuteCondition
    {
        None = 0,
        Message = 1 << 0,
        ReactionAdded = 1 << 1,
        ReactionRemoved = 1 << 2,

        AllReactions = ReactionAdded | ReactionRemoved,
        AllEvents = Message | AllReactions,
    }
}
