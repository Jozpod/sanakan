using Sanakan.DAL.Models;
using System.Collections.Generic;

namespace Sanakan.Game.Models
{
    public enum ShopType : byte
    {
        Normal = 0,
        Pvp = 1,
        Activity = 2
    }

    public static class ShopTypeExtensions
    {
        public static CardSource GetBoosterpackSource(this ShopType shopType)
        {
            return shopType switch
            {
                ShopType.Activity => CardSource.ActivityShop,
                ShopType.Pvp => CardSource.PvpShop,
                _ => CardSource.Shop,
            };
        }

        public static IList<ItemWithCost> GetItemsWithCostForShop(this ShopType shopType)
        {
            return shopType switch
            {
                ShopType.Activity => Constants.ItemsWithCostForActivityShop,
                ShopType.Pvp => Constants.ItemsWithCostForPVP,
                _ => Constants.ItemsWithCost,
            };
        }

        public static string GetShopName(this ShopType shopType)
        {
            return shopType switch
            {
                ShopType.Activity => "Kiosk",
                ShopType.Pvp => "Koszary",
                _ => "Sklepik",
            };
        }

        public static string GetShopCurrencyName(this ShopType shopType)
        {
            return shopType switch
            {
                ShopType.Activity => "AC",
                ShopType.Pvp => "PC",
                _ => "TC",
            };
        }
    }
}
