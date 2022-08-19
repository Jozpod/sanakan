using Sanakan.DAL.Models;
using System;

namespace Sanakan.Game.Extensions
{
    public static class GameDeckExtensions
    {
        private const double PVPRankMultiplier = 0.45;

        public static long CalculatePriceOfIncMaxCardCount(this GameDeck gameDeck, long count)
        {
            long price = 0;
            var basePrice = 120;
            var f = gameDeck.MaxNumberOfCards % 1000;
            var b = gameDeck.MaxNumberOfCards / 1000;
            var maxOldPriceCnt = 10 - (f / 100);
            var bExp = (b - 1) * 0.2;
            var oldPriceCnt = count;

            if (count >= maxOldPriceCnt)
            {
                oldPriceCnt = maxOldPriceCnt;
                count -= maxOldPriceCnt;
            }
            else
            {
                count = 0;
            }

            price = (long)((oldPriceCnt * basePrice) * ((b + bExp) * b));
            var rmCnt = count / 10;
            for (var i = 1; i < rmCnt + 1; i++)
            {
                bExp = (++b - 1) * 0.2;
                price += (long)((10 * basePrice) * ((b + bExp) * b));
            }

            if (count > 0)
            {
                bExp = (++b - 1) * 0.2;
                price += (long)(((count - (rmCnt * 10)) * basePrice) * ((b + rmCnt + bExp) * b));
            }

            return price;
        }

        public static bool IsNearMatchMakingRatio(this GameDeck gameDeck, GameDeck gameDeckToCompare, double margin = 0.3)
        {
            var d1MMR = gameDeck.MatchMakingRatio;
            var mDown = gameDeckToCompare.MatchMakingRatio - margin;
            var mUp = gameDeckToCompare.MatchMakingRatio + (margin * 1.2);

            return d1MMR >= mDown && d1MMR <= mUp;
        }

        public static string CalculatePVPParams(this GameDeck d1, GameDeck d2, FightResult res)
        {
            ++d1.PVPDailyGamesPlayed;

            var mmrDif = d1.MatchMakingRatio - d2.MatchMakingRatio;
            var chanceD1 = 1 / (1 + Math.Pow(10, -mmrDif / 40f));
            var chanceD2 = 1 / (1 + Math.Pow(10, mmrDif / 40f));

            var sDif = d1.SeasonalPVPRank - d2.SeasonalPVPRank;
            var sChan = 1 / (1 + Math.Pow(10, -sDif / 400f));

            var gDif = d1.GlobalPVPRank - d2.GlobalPVPRank;
            var gChan = 1 / (1 + Math.Pow(10, -gDif / 400f));

            long gRank = 0;
            long sRank = 0;

            double mmrChange = 0;
            double mmreChange = 0;

            switch (res)
            {
                case FightResult.Win:
                    ++d1.PVPWinStreak;

                    var wsb = 20 * (1 + (d1.PVPWinStreak / 10));
                    if (wsb < 20)
                    {
                        wsb = 20;
                    }

                    if (wsb > 40)
                    {
                        wsb = 40;
                    }

                    sRank = (long)(80 * (1 - sChan)) + wsb;
                    gRank = (long)(40 * (1 - gChan)) + wsb;

                    mmrChange = 2 * (1 - chanceD1);
                    mmreChange = 2 * (0 - chanceD2);
                    break;

                case FightResult.Lose:
                    d1.PVPWinStreak = 0;
                    sRank = (long)(80 * (0 - sChan));
                    gRank = (long)(40 * (0 - gChan));

                    mmrChange = 2 * (0 - chanceD1);
                    mmreChange = 2 * (1 - chanceD2);
                    break;

                case FightResult.Draw:
                    sRank = (long)(40 * (1 - sChan));
                    gRank = (long)(20 * (1 - gChan));

                    mmrChange = 1 * (1 - chanceD1);
                    mmreChange = 1 * (1 - chanceD2);
                    break;
            }

            d1.MatchMakingRatio += mmrChange;
            d2.MatchMakingRatio += mmreChange;

            d1.GlobalPVPRank += gRank;
            d1.SeasonalPVPRank += sRank;

            if (d1.GlobalPVPRank < 0)
            {
                d1.GlobalPVPRank = 0;
            }

            if (d1.SeasonalPVPRank < 0)
            {
                d1.SeasonalPVPRank = 0;
            }

            var coins = d1.GetPVPCoinsFromDuel(res);
            d1.PVPCoins += (long)coins;

            return $"**{coins:+0;-#}** PC **{gRank:+0;-#}** GR  **{sRank:+0;-#}** SR";
        }

        public static ulong GetPVPCoinsFromDuel(this GameDeck deck, FightResult fightResult)
        {
            var step = ExperienceUtils.CalculateLevel((ulong)deck.SeasonalPVPRank, PVPRankMultiplier) / 10;

            if (step > 5)
            {
                step = 5;
            }

            var coinCount = 40 + (20 * step);

            if (fightResult == FightResult.Win)
            {
                return coinCount;
            }

            return coinCount / 2;
        }
    }
}
