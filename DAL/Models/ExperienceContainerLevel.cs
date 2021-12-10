namespace Sanakan.DAL.Models
{
    public enum ExperienceContainerLevel : byte
    {
        Disabled = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4
    }

    public static class ExpContainerLevelExtensions
    {
        public static double GetMaxExpToChest(this Card card, ExperienceContainerLevel lvl)
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
                case ExperienceContainerLevel.Level4:
                    exp *= 5d;
                    break;
                case ExperienceContainerLevel.Level3:
                    exp *= 2d;
                    break;
                case ExperienceContainerLevel.Level2:
                    exp *= 1.5;
                    break;

                default:
                case ExperienceContainerLevel.Level1:
                case ExperienceContainerLevel.Disabled:
                    break;
            }

            return exp;
        }


        public static int GetMaxExpTransferToCard(this ExperienceContainerLevel level)
        {
            switch (level)
            {
                case ExperienceContainerLevel.Level1:
                    return 30;

                case ExperienceContainerLevel.Level2:
                    return 80;

                case ExperienceContainerLevel.Level3:
                    return 200;

                case ExperienceContainerLevel.Level4:
                    return -1; //unlimited

                default:
                case ExperienceContainerLevel.Disabled:
                    return 0;
            }
        }

        public static int GetMaxExpTransferToChest(this ExperienceContainerLevel level)
        {
            switch (level)
            {
                case ExperienceContainerLevel.Level1:
                    return 50;

                case ExperienceContainerLevel.Level2:
                    return 100;

                case ExperienceContainerLevel.Level3:
                case ExperienceContainerLevel.Level4:
                    return -1; //unlimited

                default:
                case ExperienceContainerLevel.Disabled:
                    return 0;
            }
        }

        public static int GetTransferCTCost(this ExperienceContainerLevel level)
        {
            switch (level)
            {
                case ExperienceContainerLevel.Level1:
                    return 8;

                case ExperienceContainerLevel.Level2:
                    return 15;

                case ExperienceContainerLevel.Level3:
                    return 10;

                case ExperienceContainerLevel.Level4:
                    return 5;

                default:
                case ExperienceContainerLevel.Disabled:
                    return 100;
            }
        }

        public static int GetChestUpgradeCostInCards(this ExperienceContainerLevel level)
        {
            switch (level)
            {
                case ExperienceContainerLevel.Disabled:
                case ExperienceContainerLevel.Level1:
                case ExperienceContainerLevel.Level2:
                    return 1;

                case ExperienceContainerLevel.Level3:
                    return 2;

                default:
                case ExperienceContainerLevel.Level4:
                    return -1; //can't upgrade
            }
        }

        public static int GetChestUpgradeCostInBlood(this ExperienceContainerLevel level)
        {
            switch (level)
            {
                case ExperienceContainerLevel.Disabled:
                    return 3;

                case ExperienceContainerLevel.Level1:
                    return 7;

                case ExperienceContainerLevel.Level2:
                    return 10;

                case ExperienceContainerLevel.Level3:
                    return 15;

                default:
                case ExperienceContainerLevel.Level4:
                    return -1; //can't upgrade
            }
        }
    }
}
