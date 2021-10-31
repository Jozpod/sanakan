namespace Sanakan.Game.Models
{
    public enum TopType
    {
        Level = 0,
        ScCnt = 1,
        TcCnt = 2,
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
        PcCnt = 14,
        AcCnt = 15
    }

    public static class TopTypeExtensions
    {
        public static string Name(this TopType type)
        {
            switch (type)
            {
                default:
                case TopType.Level:
                    return "doświadczenia";

                case TopType.ScCnt:
                    return "SC";

                case TopType.TcCnt:
                    return "TC";

                case TopType.AcCnt:
                    return "AC";

                case TopType.PcCnt:
                    return "PC";

                case TopType.Posts:
                    return "liczby wiadomości";

                case TopType.PostsMonthly:
                    return "liczby wiadomości w miesiącu";

                case TopType.PostsMonthlyCharacter:
                    return "liczby liczących się znaków na wiadomość";

                case TopType.Commands:
                    return "liczby użytych poleceń";

                case TopType.Card:
                    return "mocy karty";

                case TopType.Cards:
                    return "liczby kart";

                case TopType.CardsPower:
                    return "mocy kart";

                case TopType.Karma:
                case TopType.KarmaNegative:
                    return "karmy";

                case TopType.Pvp:
                    return "globalnego PVP";

                case TopType.PvpSeason:
                    return "miesięcznego PVP";
            }
        }
    }
}
