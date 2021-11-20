using Sanakan.DAL.Models;

namespace Sanakan.Game.Extensions
{
    public static class CardExtensions
    {
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
