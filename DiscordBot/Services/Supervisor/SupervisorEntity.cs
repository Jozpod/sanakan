using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Services.Supervisor
{
    public class SupervisorEntity
    {
        public List<SupervisorMessage> Messages { get; private set; }
        public DateTime LastMessage { get; private set; }
        public int TotalMessages { get; private set; }

        public SupervisorEntity(
            string? contentOfFirstMessage, DateTime date)
        {
            if(string.IsNullOrEmpty(contentOfFirstMessage))
            {
                Messages = new();
                TotalMessages = 1;
            }
            else
            {
                Messages.Add(new SupervisorMessage(date, contentOfFirstMessage));
                TotalMessages = 1;
            }
            
            LastMessage = date;
            
        }

        public SupervisorMessage Get(DateTime date, string content)
        {
            var msg = Messages.FirstOrDefault(x => x.Content == content);
            if (msg == null)
            {
                msg = new SupervisorMessage(date, content, 0);
                Messages.Add(msg);
            }
            return msg;
        }

        public bool IsValid(DateTime date) => (date - LastMessage).TotalMinutes <= 2;
        public void Add(SupervisorMessage message) => Messages.Add(message);
        public int Inc(DateTime date)
        {
            if ((date - LastMessage).TotalSeconds > 5)
            {
                TotalMessages = 0;
            }

            LastMessage = date;

            return ++TotalMessages;
        }
    }
}
