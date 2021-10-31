using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class ToggleCardMessage : BaseMessage
    {
        public ToggleCardMessage() : base(Priority.Low) { }

        public ulong WId { get; set; }
        public ulong DiscordUserId { get; set; }
    }
}
