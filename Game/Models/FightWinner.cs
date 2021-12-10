using Sanakan.DAL.Models;
using Sanakan.Game.Extensions;

namespace Sanakan.Game.Models
{
    public enum FightWinner : byte
    {
        Card1 = 0,
        Card2 = 1,
        Draw = 2
    }

    public static class FightWinnerExtensions
    {
        public static FightWinner GetFightWinner(Card card1, Card card2)
        {
            var FAcard1 = CardExtensions.GetFA(card1, card2);
            var FAcard2 = CardExtensions.GetFA(card2, card1);

            var c1Health = card1.GetHealthWithPenalty();
            var c2Health = card2.GetHealthWithPenalty();
            var atkTk1 = c1Health / FAcard2;
            var atkTk2 = c2Health / FAcard1;

            var winner = FightWinner.Draw;
            if (atkTk1 > atkTk2 + 0.3)
            {
                winner = FightWinner.Card1;
            }

            if (atkTk2 > atkTk1 + 0.3)
            {
                winner = FightWinner.Card2;
            }

            return winner;
        }
    }
}
