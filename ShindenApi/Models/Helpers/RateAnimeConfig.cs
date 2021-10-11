namespace Shinden.Models
{
    public class RateAnimeConfig
    {
        public AnimeRateType RateType { get; set; }
        public uint RateValue { get; set; }
        public ulong TitleId { get; set; }
    }
}
