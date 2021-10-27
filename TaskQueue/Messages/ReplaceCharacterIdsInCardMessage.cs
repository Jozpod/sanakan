using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class ReplaceCharacterIdsInCardMessage : BaseMessage
    {
        public ulong OldCharacterId { get; internal set; }
        public ulong NewCharacterId { get; internal set; }
    }
}
