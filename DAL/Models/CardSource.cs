namespace Sanakan.DAL.Models
{
    public enum CardSource : byte
    {
        Activity = 0,
        Safari = 1,
        Shop = 2,
        GodIntervention = 3,
        Api = 4,
        Other = 5,
        Migration = 6,
        PvE = 7,
        Daily = 8,
        Crafting = 9,
        PvpShop = 10,
        Figure = 11,
        Expedition = 12,
        ActivityShop = 13
    }

    public static class CardSourceExtensions
    {
        public static string GetString(this CardSource source) => source switch
        {
            CardSource.Activity => "Aktywność",
            CardSource.Safari => "Safari",
            CardSource.Shop => "Sklepik",
            CardSource.GodIntervention => "Czity",
            CardSource.Api => "Strona",
            CardSource.Migration => "Stara baza",
            CardSource.PvE => "Walki na boty",
            CardSource.Daily => "Karta+",
            CardSource.Crafting => "Tworzenie",
            CardSource.PvpShop => "Koszary",
            CardSource.Figure => "Figurka",
            CardSource.Expedition => "Wyprawa",
            CardSource.ActivityShop => "Kiosk",
            _ => "Inne",
        };
    }
}
