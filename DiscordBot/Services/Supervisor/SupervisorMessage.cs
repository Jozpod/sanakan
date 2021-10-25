using System;

namespace Sanakan.Services.Supervisor
{
    public class SupervisorMessage
    {
        public SupervisorMessage(
            DateTime previousOccurrence,
            string content,
            int count = 1)
        {
            PreviousOccurrence = previousOccurrence;
            Content = content;
            Count = count;
        }

        public DateTime PreviousOccurrence { get; private set; }
        public string Content { get; private set; }
        public int Count { get; private set; }

        public bool IsValid(DateTime dateTime) => (dateTime - PreviousOccurrence).TotalMinutes <= 1;
        public int Inc() => ++Count;
    }
}
