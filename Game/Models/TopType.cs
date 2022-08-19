namespace Sanakan.Game.Models
{
    public enum TopType : byte
    {
        Level = 0,
        ScCount = 1,
        TcCount = 2,
        Posts = 3,
        PostsMonthly = 4,
        PostsMonthlyCharacter = 5,
        Commands = 6,
        Cards = 7,
        CardsPower = 8,
        Card = 9,
        Karma = 10,
        KarmaNegative = 11,
        Pvp = 12,
        PvpSeason = 13,
        PcCount = 14,
        AcCount = 15
    }

    public static class TopTypeExtensions
    {
        public static string Name(this TopType type) => type switch
        {
            TopType.ScCount => "SC",
            TopType.TcCount => "TC",
            TopType.AcCount => "AC",
            TopType.PcCount => "PC",
            TopType.Posts => "liczby wiadomości",
            TopType.PostsMonthly => "liczby wiadomości w miesiącu",
            TopType.PostsMonthlyCharacter => "liczby liczących się znaków na wiadomość",
            TopType.Commands => "liczby użytych poleceń",
            TopType.Card => "mocy karty",
            TopType.Cards => "liczby kart",
            TopType.CardsPower => "mocy kart",
            TopType.Karma or TopType.KarmaNegative => "karmy",
            TopType.Pvp => "globalnego PVP",
            TopType.PvpSeason => "miesięcznego PVP",
            _ => "doświadczenia",
        };
    }
}
