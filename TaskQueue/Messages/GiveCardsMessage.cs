using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.TaskQueue.Messages
{
    public class GiveCardsMessage : BaseMessage
    {
        public GiveCardsMessage() : base(Priority.Low) { }

        public ulong DiscordUserId { get; set; }

        public IEnumerable<BoosterPack> BoosterPacks { get; set; } = Enumerable.Empty<BoosterPack>();
    }
}
