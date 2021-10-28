using Sanakan.DAL.Models;
using System.Collections.Generic;

namespace Sanakan.TaskQueue.Messages
{
    public class OpenCardsMessage : BaseMessage
    {
        public OpenCardsMessage() : base(Priority.Low) { }

        public List<Card> Cards { get; set; }
        public ulong DiscordUserId { get; set; }
    }
}
