using Sanakan.DAL.Models;
using Sanakan.Game;
using Sanakan.Game.Models;

namespace DiscordBot.Services.PocketWaifu
{
    public enum ShopType
    {
        Normal = 0,
        Pvp = 1,
        Activity = 2
    }

    public static class ShopTypeExtensions
    {
        public static CardSource GetBoosterpackSource(this ShopType shopType)
        {
            switch (shopType)
            {
                case ShopType.Activity:
                    return CardSource.ActivityShop;

                case ShopType.Pvp:
                    return CardSource.PvpShop;

                default:
                case ShopType.Normal:
                    return CardSource.Shop;
            }
        }

        public static ItemWithCost[] GetItemsWithCostForShop(this ShopType shopType)
        {
            switch (shopType)
            {
                case ShopType.Activity:
                    return Constants.ItemsWithCostForActivityShop;

                case ShopType.Pvp:
                    return Constants.ItemsWithCostForPVP;

                case ShopType.Normal:
                default:
                    return Constants.ItemsWithCost;
            }
        }

        public static string GetShopName(this ShopType shopType)
        {
            switch (shopType)
            {
                case ShopType.Activity:
                    return "Kiosk";

                case ShopType.Pvp:
                    return "Koszary";

                case ShopType.Normal:
                default:
                    return "Sklepik";
            }
        }

        public static string GetShopCurrencyName(this ShopType shopType)
        {
            switch (shopType)
            {
                case ShopType.Activity:
                    return "AC";

                case ShopType.Pvp:
                    return "PC";

                case ShopType.Normal:
                default:
                    return "TC";
            }
        }
    }
}
