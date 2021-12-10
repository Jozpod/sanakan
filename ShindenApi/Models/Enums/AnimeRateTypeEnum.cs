namespace Sanakan.ShindenApi.Models.Enums
{
    public enum AnimeRateType : byte
    {
        Characters = 0,
        Graphic = 1,
        Music = 2,
        Stroy = 3,
        Total = 4
    }

    public static class AnimeRateTypeExtensions
    {
        public static string ToQuery(this AnimeRateType type)
        {
            switch (type)
            {
                case AnimeRateType.Characters: return "titlecahracters";
                case AnimeRateType.Graphic: return "graphics";
                case AnimeRateType.Music: return "music";
                case AnimeRateType.Stroy: return "story";
                case AnimeRateType.Total: return "total";
                default: return "total";
            }
        }
    }
}