namespace Shinden.Models
{
    public class RateMangaConfig
    {
        public MangaRateType RateType { get; set; }
        public uint RateValue { get; set; }
        public ulong TitleId { get; set; }
    }
}
