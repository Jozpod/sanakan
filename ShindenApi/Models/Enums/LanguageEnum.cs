namespace Sanakan.ShindenApi.Models.Enums
{
    public enum Language : byte
    {
        NotSpecified = 0,
        Japanese = 1,
        Chinese = 2,
        English = 3,
        Korean = 4,
        Polish = 5,
    }

    public static class LanguageExtensions
    {
        public static string ToName(this Language lang)
        {
            switch (lang)
            {
                case Language.English: return "Angielski";
                case Language.Korean: return "Koreański";
                case Language.Chinese: return "Chiński";
                case Language.Polish: return "Polski";

                case Language.NotSpecified:
                default: return "--";
            }
        }
    }
}
