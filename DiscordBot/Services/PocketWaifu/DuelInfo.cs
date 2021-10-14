using DiscordBot.Services.PocketWaifu;
using Sanakan.DAL.Models;

namespace Sanakan.Services.PocketWaifu
{
    public class DuelInfo
    {
        public WinnerSide Side { get; set; }
        public Card Winner { get; set; }
        public Card Loser { get; set; }
    }
}