using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    public class FightHistory
    {
        public FightHistory(PlayerInfo? winner)
        {
            Winner = winner;
            Rounds = new List<RoundInfo>();
        }

        public PlayerInfo? Winner { get; set; }

        public IList<RoundInfo> Rounds { get; set; } = new List<RoundInfo>();
    }
}
