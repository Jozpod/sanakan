using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    public class RoundInfo
    {
        public RoundInfo()
        {
            Cards = new List<HpSnapshot>();
            Fights = new List<AttackInfo>();
        }

        public List<HpSnapshot> Cards { get; set; }

        public List<AttackInfo> Fights { get; set; }
    }
}
