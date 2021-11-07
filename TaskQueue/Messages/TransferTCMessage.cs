﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class TransferTCMessage : BaseMessage
    {
        public TransferTCMessage() : base(Priority.Low) { }

        public ulong Amount { get; set; }
        public ulong DiscordUserId { get; set; }
        public ulong ShindenUserId { get; set; }
    }
}