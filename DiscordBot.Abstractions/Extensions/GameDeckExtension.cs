using Discord;
using DiscordBot.Services;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Extensions
{

    public static class GameDeckExtension
    {
        public const double MAX_DECK_POWER = 800;
        public const double MIN_DECK_POWER = 200;

    
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
            else count = 0;

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
    }
}
