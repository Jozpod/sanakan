using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Game
{
    [ExcludeFromCodeCoverage]
    public static class Constants
    {
        public static ItemWithCost[] ItemsWithCost = new[]
        {
            new ItemWithCost(3,     ItemType.AffectionRecoverySmall.ToItem()),
            new ItemWithCost(14,    ItemType.AffectionRecoveryNormal.ToItem()),
            new ItemWithCost(109,   ItemType.AffectionRecoveryBig.ToItem()),
            new ItemWithCost(29,    ItemType.DereReRoll.ToItem()),
            new ItemWithCost(79,    ItemType.CardParamsReRoll.ToItem()),
            new ItemWithCost(1099,  ItemType.IncreaseUpgradeCount.ToItem()),
            new ItemWithCost(69,    ItemType.ChangeCardImage.ToItem()),
            new ItemWithCost(999,   ItemType.SetCustomImage.ToItem()),
            new ItemWithCost(659,   ItemType.SetCustomBorder.ToItem()),
            new ItemWithCost(149,   ItemType.ChangeStarType.ToItem()),
            new ItemWithCost(99,    ItemType.RandomBoosterPackSingleE.ToItem()),
            new ItemWithCost(999,   ItemType.BigRandomBoosterPackE.ToItem()),
            new ItemWithCost(1199,  ItemType.RandomTitleBoosterPackSingleE.ToItem()),
            new ItemWithCost(199,   ItemType.RandomNormalBoosterPackB.ToItem()),
            new ItemWithCost(499,   ItemType.RandomNormalBoosterPackA.ToItem()),
            new ItemWithCost(899,   ItemType.RandomNormalBoosterPackS.ToItem()),
            new ItemWithCost(1299,  ItemType.RandomNormalBoosterPackSS.ToItem()),
            new ItemWithCost(569,   ItemType.ResetCardValue.ToItem()),
        };

        public static ItemWithCost[] ItemsWithCostForPVP = new []
        {
            new ItemWithCost(169,    ItemType.AffectionRecoveryNormal.ToItem()),
            new ItemWithCost(1699,   ItemType.IncreaseExpBig.ToItem()),
            new ItemWithCost(1699,   ItemType.CheckAffection.ToItem()),
            new ItemWithCost(16999,  ItemType.IncreaseUpgradeCount.ToItem()),
            new ItemWithCost(46999,  ItemType.BetterIncreaseUpgradeCnt.ToItem()),
            new ItemWithCost(4699,   ItemType.ChangeCardImage.ToItem()),
            new ItemWithCost(269999, ItemType.SetCustomImage.ToItem()),
        };

        public static ItemWithCost[] ItemsWithCostForActivityShop = new[]
        {
            new ItemWithCost(6,     ItemType.AffectionRecoveryBig.ToItem()),
            new ItemWithCost(65,    ItemType.IncreaseExpBig.ToItem()),
            new ItemWithCost(500,   ItemType.IncreaseUpgradeCount.ToItem()),
            new ItemWithCost(1800,  ItemType.SetCustomImage.ToItem()),
            new ItemWithCost(150,   ItemType.RandomBoosterPackSingleE.ToItem()),
            new ItemWithCost(1500,  ItemType.BigRandomBoosterPackE.ToItem()),
        };

        /// <summary>
        /// Initial sample of characters used in free card retrieval.
        /// </summary>
        /// <remarks>Link to character in shinden: https://shinden.pl/character/character_id</remarks>
        public static List<ulong> CharacterTitleIds = new()
        {
            7431, // Nozomi Kasuga - Harukanaru Toki no Naka de 3
            50646, // Anderson - Takarajima
            10831, // Kamuro Ishigami - Kamikaze
            54081, // Ryunosuke Sasaki - Nisekoi
            53776, // Mion Hinomiya - Akarui Sekai Keikaku
            12434, // Shijima Kurookano - Nabari no Ou
            44867, // Eriko - Pokemon Advanced Generation
            51100, // Milla Bassett - Seirei Tsukai no Blade Dance
            4961, // Guld Goa Bowman - Macross Plus
            55260, // Fecchan - Nichijou
            53382, // Haruaki Yachi - C³
            53685, // Sophie - Gosick
            35405, // Fuugi -  MÄR
            54195, // Syon Galva - Wild Fangs
            2763, // Saotome Masami - Boogiepop wa Warawanai
            43864, // Ryuuichirou Hoshi - Doujin Work
            52427, // Benisuzume - Sidonia no Kishi
            52111, // Eriko Tanaka - Route 225
            53257, // Shin Nitta - Velvet Kiss
            45085 // Akira - Minori Scramble!
        };

        public static List<RarityChance> RarityChances = new List<RarityChance>()
            {
                new RarityChance(5,    Rarity.SS),
                new RarityChance(25,   Rarity.S ),
                new RarityChance(75,   Rarity.A ),
                new RarityChance(175,  Rarity.B ),
                new RarityChance(370,  Rarity.C ),
                new RarityChance(650,  Rarity.D ),
                new RarityChance(1000, Rarity.E ),
            };

        public const int DERE_TAB_SIZE = ((int)Dere.Yato) + 1;

        public static double[,] DereDmgRelation = new double[DERE_TAB_SIZE, DERE_TAB_SIZE]
        {
            //Tsundere, Kamidere, Deredere, Yandere, Dandere, Kuudere, Mayadere, Bodere, Yami, Raito, Yato
            { 0.5,      2,        2,        2,       2,       2,       2,        2,      3,    3,     3     }, //Tsundere
            { 1,        0.5,      2,        0.5,     1,       1,       1,        1,      2,    1,     2     }, //Kamidere
            { 1,        1,        0.5,      2,       0.5,     1,       1,        1,      2,    1,     2     }, //Deredere
            { 1,        1,        1,        0.5,     2,       0.5,     1,        1,      2,    1,     2     }, //Yandere
            { 1,        1,        1,        1,       0.5,     2,       0.5,      1,      2,    1,     2     }, //Dandere
            { 1,        1,        1,        1,       1,       0.5,     2,        0.5,    2,    1,     2     }, //Kuudere
            { 1,        0.5,      1,        1,       1,       1,       0.5,      2,      2,    1,     2     }, //Mayadere
            { 1,        2,        0.5,      1,       1,       1,       1,        0.5,    2,    1,     2     }, //Bodere
            { 1,        1,        1,        1,       1,       1,       1,        1,      0.5,  3,     2     }, //Yami
            { 0.5,      0.5,      0.5,      0.5,     0.5,     0.5,     0.5,      0.5,    3,    0.5,   1     }, //Raito
            { 0.5,      0.5,      0.5,      0.5,     0.5,     0.5,     0.5,      0.5,    1,    0.5,   1     }, //Yato
        };

        public static IDictionary<ExpeditionCardType, Dictionary<ItemType, (int, int)>> ChanceOfItemsInExpedition = 
            new Dictionary<ExpeditionCardType, Dictionary<ItemType, (int, int)>>
        {
            {ExpeditionCardType.NormalItemWithExp, new Dictionary<ItemType, (int, int)>
                {
                    {ItemType.AffectionRecoverySmall,   (0,    4049)},
                    {ItemType.AffectionRecoveryNormal,  (4049, 6949)},
                    {ItemType.DereReRoll,               (6949, 7699)},
                    {ItemType.CardParamsReRoll,         (7699, 8559)},
                    {ItemType.AffectionRecoveryBig,     (8559, 9419)},
                    {ItemType.AffectionRecoveryGreat,   (9419, 9729)},
                    {ItemType.IncreaseUpgradeCount,     (9729, 9769)},
                    {ItemType.IncreaseExpSmall,         (9769, 10000)},
                    {ItemType.IncreaseExpBig,           (-1,   -2)},
                    {ItemType.BetterIncreaseUpgradeCnt, (-3,   -4)},
                }
            },
            {ExpeditionCardType.ExtremeItemWithExp, new Dictionary<ItemType, (int, int)>
                {
                    {ItemType.AffectionRecoverySmall,   (-1,   -2)},
                    {ItemType.AffectionRecoveryNormal,  (0,    3499)},
                    {ItemType.DereReRoll,               (-3,   -4)},
                    {ItemType.CardParamsReRoll,         (-5,   -6)},
                    {ItemType.AffectionRecoveryBig,     (3499, 6299)},
                    {ItemType.AffectionRecoveryGreat,   (6299, 7499)},
                    {ItemType.IncreaseUpgradeCount,     (7499, 7799)},
                    {ItemType.IncreaseExpSmall,         (7799, 8599)},
                    {ItemType.IncreaseExpBig,           (8599, 9799)},
                    {ItemType.BetterIncreaseUpgradeCnt, (9799, 10000)},
                }
            },
            {ExpeditionCardType.DarkItems, new Dictionary<ItemType, (int, int)>
                {
                    {ItemType.AffectionRecoverySmall,   (0,    1999)},
                    {ItemType.AffectionRecoveryNormal,  (1999, 5999)},
                    {ItemType.DereReRoll,               (-1,   -2)},
                    {ItemType.CardParamsReRoll,         (5999, 6449)},
                    {ItemType.AffectionRecoveryBig,     (6449, 8149)},
                    {ItemType.AffectionRecoveryGreat,   (8149, 8949)},
                    {ItemType.IncreaseUpgradeCount,     (8949, 9049)},
                    {ItemType.IncreaseExpSmall,         (9049, 9849)},
                    {ItemType.IncreaseExpBig,           (-2,   -3)},
                    {ItemType.BetterIncreaseUpgradeCnt, (9849, 10000)},
                }
            },
            {ExpeditionCardType.DarkItemWithExp, new Dictionary<ItemType, (int, int)>
                {
                    {ItemType.AffectionRecoverySmall,   (0,    2499)},
                    {ItemType.AffectionRecoveryNormal,  (2499, 5999)},
                    {ItemType.DereReRoll,               (5999, 6999)},
                    {ItemType.CardParamsReRoll,         (6999, 7199)},
                    {ItemType.AffectionRecoveryBig,     (7199, 8499)},
                    {ItemType.AffectionRecoveryGreat,   (8499, 9099)},
                    {ItemType.IncreaseUpgradeCount,     (9099, 9199)},
                    {ItemType.IncreaseExpSmall,         (-1,   -2)},
                    {ItemType.IncreaseExpBig,           (9199,  10000)},
                    {ItemType.BetterIncreaseUpgradeCnt, (-3,   -4)},
                }
            },
            {ExpeditionCardType.LightItems, new Dictionary<ItemType, (int, int)>
                {
                    {ItemType.AffectionRecoverySmall,   (0,    3799)},
                    {ItemType.AffectionRecoveryNormal,  (3799, 6699)},
                    {ItemType.DereReRoll,               (-1,   -2)},
                    {ItemType.CardParamsReRoll,         (6699, 7199)},
                    {ItemType.AffectionRecoveryBig,     (7199, 8199)},
                    {ItemType.AffectionRecoveryGreat,   (8199, 8699)},
                    {ItemType.IncreaseUpgradeCount,     (8699, 8799)},
                    {ItemType.IncreaseExpSmall,         (8799, 9899)},
                    {ItemType.IncreaseExpBig,           (-2,   -3)},
                    {ItemType.BetterIncreaseUpgradeCnt, (9899, 10000)},
                }
            },
            {ExpeditionCardType.LightItemWithExp, new Dictionary<ItemType, (int, int)>
                {
                    {ItemType.AffectionRecoverySmall,   (0,    3799)},
                    {ItemType.AffectionRecoveryNormal,  (3799, 6399)},
                    {ItemType.DereReRoll,               (6399, 7399)},
                    {ItemType.CardParamsReRoll,         (7399, 7899)},
                    {ItemType.AffectionRecoveryBig,     (7899, 8899)},
                    {ItemType.AffectionRecoveryGreat,   (8899, 9399)},
                    {ItemType.IncreaseUpgradeCount,     (9399, 9499)},
                    {ItemType.IncreaseExpSmall,         (-1,   -2)},
                    {ItemType.IncreaseExpBig,           (9499, 10000)},
                    {ItemType.BetterIncreaseUpgradeCnt, (-3,   -4)},
                }
            }
        };

        public static IDictionary<ExpeditionCardType, Dictionary<EventType, (int, int)>> ChanceOfEvent =
           new Dictionary<ExpeditionCardType, Dictionary<EventType, (int, int)>>
       {
            {ExpeditionCardType.NormalItemWithExp, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (0,    1500)},
                    {EventType.MoreExperience,      (1500, 3900)},
                    {EventType.IncreaseAttack,      (3900, 7400)},
                    {EventType.IncreaseDefence,     (7400, 10000)},
                    {EventType.AddReset,            (-1,   -2)},
                    {EventType.NewCard,             (-3,   -4)},
                    {EventType.ChangeDere,          (-5,   -6)},
                    {EventType.DecreaseAttack,      (-7,   -8)},
                    {EventType.DecreaseDefence,     (-9,   -10)},
                    {EventType.DecreaseAffection,   (-11,  -12)},
                    {EventType.LoseCard,            (-13,  -14)},
                    {EventType.Fight,               (-15,  -16)},
                }
            },
            {ExpeditionCardType.ExtremeItemWithExp, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (0,    900)},
                    {EventType.MoreExperience,      (900,  1900)},
                    {EventType.IncreaseAttack,      (1900, 3000)},
                    {EventType.IncreaseDefence,     (3000, 4000)},
                    {EventType.AddReset,            (4000, 4200)},
                    {EventType.NewCard,             (4200, 4300)},
                    {EventType.ChangeDere,          (4300, 5000)},
                    {EventType.DecreaseAttack,      (5000, 6200)},
                    {EventType.DecreaseDefence,     (6200, 7400)},
                    {EventType.DecreaseAffection,   (7400, 9000)},
                    {EventType.LoseCard,            (9000, 10000)},
                    {EventType.Fight,               (-1,  -2)},
                }
            },
            {ExpeditionCardType.DarkItemWithExp, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (0,    1000)},
                    {EventType.MoreExperience,      (1000, 2500)},
                    {EventType.IncreaseAttack,      (2500, 5000)},
                    {EventType.IncreaseDefence,     (5000, 7000)},
                    {EventType.AddReset,            (-1,   -2)},
                    {EventType.Fight,               (7000, 7300)},
                    {EventType.ChangeDere,          (7300, 7900)},
                    {EventType.DecreaseAttack,      (7900, 8500)},
                    {EventType.DecreaseDefence,     (8500, 9000)},
                    {EventType.DecreaseAffection,   (9000, 10000)},
                    {EventType.LoseCard,            (-3,   -4)},
                    {EventType.NewCard,             (-5,   -6)},
                }
            },
            {ExpeditionCardType.LightItemWithExp, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (0,    1000)},
                    {EventType.MoreExperience,      (1000, 2500)},
                    {EventType.IncreaseAttack,      (2500, 5000)},
                    {EventType.IncreaseDefence,     (5000, 7000)},
                    {EventType.AddReset,            (-1,   -2)},
                    {EventType.Fight,               (7000, 7300)},
                    {EventType.ChangeDere,          (7300, 7900)},
                    {EventType.DecreaseAttack,      (7900, 8500)},
                    {EventType.DecreaseDefence,     (8500, 9000)},
                    {EventType.DecreaseAffection,   (9000, 10000)},
                    {EventType.LoseCard,            (-3,   -4)},
                    {EventType.NewCard,             (-5,   -6)},
                }
            },
            {ExpeditionCardType.DarkItems, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (-1,   -2)},
                    {EventType.MoreExperience,      (-3,   -4)},
                    {EventType.IncreaseAttack,      (0,    2200)},
                    {EventType.IncreaseDefence,     (2200, 4100)},
                    {EventType.AddReset,            (-5,   -6)},
                    {EventType.Fight,               (4100, 4400)},
                    {EventType.ChangeDere,          (4400, 5400)},
                    {EventType.DecreaseAttack,      (5400, 6600)},
                    {EventType.DecreaseDefence,     (6600, 8000)},
                    {EventType.DecreaseAffection,   (8000, 10000)},
                    {EventType.LoseCard,            (-7,   -8)},
                    {EventType.NewCard,             (-9,   -10)},
                }
            },
            {ExpeditionCardType.LightItems, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (-1,   -2)},
                    {EventType.MoreExperience,      (-3,   -4)},
                    {EventType.IncreaseAttack,      (0,    2200)},
                    {EventType.IncreaseDefence,     (2200, 4100)},
                    {EventType.AddReset,            (-5,   -6)},
                    {EventType.Fight,               (4100, 4400)},
                    {EventType.ChangeDere,          (4400, 5400)},
                    {EventType.DecreaseAttack,      (5400, 6600)},
                    {EventType.DecreaseDefence,     (6600, 8000)},
                    {EventType.DecreaseAffection,   (8000, 10000)},
                    {EventType.LoseCard,            (-7,   -8)},
                    {EventType.NewCard,             (-9,   -10)},
                }
            },
            {ExpeditionCardType.DarkExp, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (-1,   -2)},
                    {EventType.MoreExperience,      (-3,   -4)},
                    {EventType.IncreaseAttack,      (0,    2200)},
                    {EventType.IncreaseDefence,     (2200, 4100)},
                    {EventType.AddReset,            (-5,   -6)},
                    {EventType.Fight,               (4100, 4400)},
                    {EventType.ChangeDere,          (4400, 5400)},
                    {EventType.DecreaseAttack,      (5400, 6600)},
                    {EventType.DecreaseDefence,     (6600, 8000)},
                    {EventType.DecreaseAffection,   (8000, 10000)},
                    {EventType.LoseCard,            (-7,   -8)},
                    {EventType.NewCard,             (-9,   -10)},
                }
            },
            {ExpeditionCardType.LightExp, new Dictionary<EventType, (int, int)>
                {
                    {EventType.MoreItems,           (-1,   -2)},
                    {EventType.MoreExperience,      (-3,   -4)},
                    {EventType.IncreaseAttack,      (0,    2200)},
                    {EventType.IncreaseDefence,     (2200, 4100)},
                    {EventType.AddReset,            (-5,   -6)},
                    {EventType.Fight,               (4100, 4400)},
                    {EventType.ChangeDere,          (4400, 5400)},
                    {EventType.DecreaseAttack,      (5400, 6600)},
                    {EventType.DecreaseDefence,     (6600, 8000)},
                    {EventType.DecreaseAffection,   (8000, 10000)},
                    {EventType.LoseCard,            (-7,   -8)},
                    {EventType.NewCard,             (-9,   -10)},
                }
            }
       };
    }
}
