using Sanakan.DAL.Models;

namespace Sanakan.Game.Models
{
    public record CardWithHealth
    {
        public Card Card { get; set; } = null;

        public double Health { get; set; }
    }
}
