using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class DeleteUserMessage : BaseMessage
    {
        public DeleteUserMessage() : base(Priority.High)
        {
        }

        public ulong DiscordUserId { get; set; }
    }
}
