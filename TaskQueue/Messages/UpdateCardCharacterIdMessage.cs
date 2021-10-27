using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class UpdateCardCharacterIdMessage : BaseMessage
    {
        public UpdateCardCharacterIdMessage() : base(Priority.Low) { }

        public ulong CharacterId { get; set; }
    }
}
