using Sanakan.Game.Models;
using System.Collections.Generic;

namespace Sanakan.TaskQueue.Messages
{
    public class GivesCardsMessage : BaseMessage
    {
        public GivesCardsMessage() : base(Priority.Low) { }

        public ulong CharacterId { get; set; }
        public ulong DiscordUserId { get; set; }
        public List<CardBoosterPack> BoosterPacks { get; set; }
    }
}
