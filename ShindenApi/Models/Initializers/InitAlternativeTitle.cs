namespace Shinden.Models.Initializers
{
    public class InitAlternativeTitle
    {
        public ulong Id { get; set; }
        public ulong? TitleId { get; set; }
        public string Content { get; set; }
        public bool? IsAccepted { get; set; }
        public Language Language { get; set; }
        public AlternativeTitleType Type { get; set; }
    }
}