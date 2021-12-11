using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System.Collections.Generic;

namespace Sanakan.Game.Extensions
{
    public static class CardExtensions
    {
        public static CardRarityStats GetRarityStats(this IEnumerable<Card> cards)
        {
            var cardRarityStats = new CardRarityStats();

            foreach (var card in cards)
            {
                switch (card.Rarity)
                {
                    case Rarity.A:
                        cardRarityStats.A++;
                        break;
                    case Rarity.B:
                        cardRarityStats.B++;
                        break;
                    case Rarity.C:
                        cardRarityStats.C++;
                        break;
                    case Rarity.D:
                        cardRarityStats.D++;
                        break;
                    case Rarity.E:
                        cardRarityStats.E++;
                        break;
                    case Rarity.S:
                        cardRarityStats.S++;
                        break;
                    case Rarity.SS:
                        cardRarityStats.SS++;
                        break;
                    case Rarity.SSS:
                        cardRarityStats.SSS++;
                        break;
                }
            }

            return cardRarityStats;
        }

        public static double GetFA(Card target, Card enemy)
        {
            double atk1 = target.GetAttackWithBonus();

            if (!target.HasImage())
            {
                atk1 -= atk1 * 20 / 100;
            }

            double def2 = enemy.GetDefenceWithBonus();
            if (!enemy.HasImage())
            {
                def2 -= def2 * 20 / 100;
            }

            var realAtk1 = atk1 - def2;
            if (!target.FromFigure || !enemy.FromFigure)
            {
                if (def2 > 99)
                {
                    def2 = 99;
                }
                realAtk1 = atk1 * (100 - def2) / 100;
            }

            realAtk1 *= Game.Constants.DereDmgRelation[(int)target.Dere, (int)enemy.Dere];

            return realAtk1;
        }
    }
}
