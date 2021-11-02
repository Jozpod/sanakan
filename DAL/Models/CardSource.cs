namespace Sanakan.DAL.Models
{
    public enum CardSource
    {
        Activity,
        Safari,
        Shop,
        GodIntervention,
        Api,
        Other,
        Migration,
        PvE,
        Daily,
        Crafting,
        PvpShop,
        Figure,
        Expedition,
        ActivityShop
    }

    public static class CardSourceExtensions
    {
        public static string GetString(this CardSource source)
        {
            switch (source)
            {
                case CardSource.Activity: return "Aktywność";
                case CardSource.Safari: return "Safari";
                case CardSource.Shop: return "Sklepik";
                case CardSource.GodIntervention: return "Czity";
                case CardSource.Api: return "Strona";
                case CardSource.Migration: return "Stara baza";
                case CardSource.PvE: return "Walki na boty";
                case CardSource.Daily: return "Karta+";
                case CardSource.Crafting: return "Tworzenie";
                case CardSource.PvpShop: return "Koszary";
                case CardSource.Figure: return "Figurka";
                case CardSource.Expedition: return "Wyprawa";
                case CardSource.ActivityShop: return "Kiosk";

                default:
                case CardSource.Other: return "Inne";
            }
        }
    }
}
