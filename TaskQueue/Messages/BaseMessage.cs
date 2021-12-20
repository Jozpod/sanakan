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

        public int CompareTo(BaseMessage? other) => other switch
        {
            null => -1,
            var message when message.Priority > Priority => 1,
            var message when message.Priority < Priority => -1,
            _ => 0
        };
    }
}
