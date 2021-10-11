namespace Shinden.Models.Initializers
{
    public class InitListStats
    {
        public IReadSpeed ReadSpeed { get; set;}
        public ulong? ChaptersCount { get; set; }
        public ulong? EpisodesCount { get; set; }
        public IDedicatedTime MangaTime { get; set; }
        public IDedicatedTime AnimeTime { get; set; }
        public ulong? ReadedMoreThanOnce { get; set; }
        public ulong? WachedMoreThanOnce { get; set; }
        public IMeanScore AnimeMeanScore { get; set; }
        public IMeanScore MangaMeanScore { get; set; }
        public ISeriesStatus AnimeStatus { get; set; }
        public ISeriesStatus MangaStatus { get; set; }
    }
}