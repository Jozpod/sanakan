﻿namespace Sanakan.TaskQueue.Messages
{
    public class ToggleCardMessage : BaseMessage
    {
        public ToggleCardMessage()
            : base(Priority.Low)
        {
        }

        public ulong CardId { get; set; }

        public ulong DiscordUserId { get; set; }
    }
}
