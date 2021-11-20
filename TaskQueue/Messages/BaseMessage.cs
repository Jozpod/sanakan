using System;

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
            if(other == null)
            {
                return -1;
            }

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
