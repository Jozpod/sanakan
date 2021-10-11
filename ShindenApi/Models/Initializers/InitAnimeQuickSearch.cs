namespace Shinden.Models.Initializers
{
    public class InitAnimeQuickSearch
    {
        public string Title { get; set; }
        public ulong AnimeId { get; set; }
        public ulong? CoverId { get; set; }
        public AnimeType Type { get; set; }
        public AnimeStatus Status { get; set; }
    }
}
