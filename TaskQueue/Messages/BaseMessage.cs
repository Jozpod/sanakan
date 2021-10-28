using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public abstract class BaseMessage : IComparable<BaseMessage>
    {
        public Priority Priority { get; }

        public BaseMessage(Priority priority)
        {
            Priority = priority;
        }

        public int CompareTo(BaseMessage other)
        {
            if(Priority < other.Priority)
            {
                return 1;
            }

            if (Priority > other.Priority)
            {
                return -1;
            }

            return 0;
        }
    }
}
