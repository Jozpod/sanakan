using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System.Collections.Generic;

namespace Sanakan.TaskQueue.Messages
{
    public class GiveCardsMessage : BaseMessage
    {
        public GiveCardsMessage() : base(Priority.Low) { }

        public ulong CharacterId { get; set; }
        public ulong DiscordUserId { get; set; }
        public IEnumerable<BoosterPack> BoosterPacks { get; set; }
    }
}
