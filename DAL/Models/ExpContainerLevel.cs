namespace Sanakan.DAL.Models
{
    public enum ExpContainerLevel
    {
        Disabled = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4
    }

    public static class ExpContainerLevelExtensions
    {
        public static double GetMaxExpToChest(this Card card, ExpContainerLevel lvl)
        {
            double exp = 0;

            switch (card.Rarity)
            {
                case Rarity.SSS:
                    exp = 16d;
                    break;

                case Rarity.SS:
                    exp = 8d;
                    break;

                case Rarity.S:
                    exp = 4.8;
                    break;

                case Rarity.A:
                case Rarity.B:
                    exp = 3.5;
                    break;

                case Rarity.C:
                    exp = 2.5;
                    break;

                default:
                case Rarity.D:
                case Rarity.E:
                    exp = 1.5;
                    break;
            }

            switch (lvl)
            {
                case ExpContainerLevel.Level4:
                    exp *= 5d;
                    break;
                case ExpContainerLevel.Level3:
                    exp *= 2d;
                    break;
                case ExpContainerLevel.Level2:
                    exp *= 1.5;
                    break;

                default:
                case ExpContainerLevel.Level1:
                case ExpContainerLevel.Disabled:
                    break;
            }

            return exp;
        }


        public static int GetMaxExpTransferToCard(this ExpContainerLevel level)
        {
            switch (level)
            {
                case ExpContainerLevel.Level1:
                    return 30;

                case ExpContainerLevel.Level2:
                    return 80;

                case ExpContainerLevel.Level3:
                    return 200;

                case ExpContainerLevel.Level4:
                    return -1; //unlimited

                default:
                case ExpContainerLevel.Disabled:
                    return 0;
            }
        }

        public static int GetMaxExpTransferToChest(this ExpContainerLevel level)
        {
            switch (level)
            {
                case ExpContainerLevel.Level1:
                    return 50;

                case ExpContainerLevel.Level2:
                    return 100;

                case ExpContainerLevel.Level3:
                case ExpContainerLevel.Level4:
                    return -1; //unlimited

                default:
                case ExpContainerLevel.Disabled:
                    return 0;
            }
        }

        public static int GetTransferCTCost(this ExpContainerLevel level)
        {
            switch (level)
            {
                case ExpContainerLevel.Level1:
                    return 8;

                case ExpContainerLevel.Level2:
                    return 15;

                case ExpContainerLevel.Level3:
                    return 10;

                case ExpContainerLevel.Level4:
                    return 5;

                default:
                case ExpContainerLevel.Disabled:
                    return 100;
            }
        }

        public static int GetChestUpgradeCostInCards(this ExpContainerLevel level)
        {
            switch (level)
            {
                case ExpContainerLevel.Disabled:
                case ExpContainerLevel.Level1:
                case ExpContainerLevel.Level2:
                    return 1;

                case ExpContainerLevel.Level3:
                    return 2;

                default:
                case ExpContainerLevel.Level4:
                    return -1; //can't upgrade
            }
        }

        public static int GetChestUpgradeCostInBlood(this ExpContainerLevel level)
        {
            switch (level)
            {
                case ExpContainerLevel.Disabled:
                    return 3;

                case ExpContainerLevel.Level1:
                    return 7;

                case ExpContainerLevel.Level2:
                    return 10;

                case ExpContainerLevel.Level3:
                    return 15;

                default:
                case ExpContainerLevel.Level4:
                    return -1; //can't upgrade
            }
        }
    }
}
