using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class ListStats : IListStats
    {
        public ListStats(InitListStats Init)
        {
            ReadSpeed = Init.ReadSpeed;
            AnimeTime = Init.AnimeTime;
            MangaTime = Init.MangaTime;
            ReadSpeed = Init.ReadSpeed;
            MangaStatus = Init.MangaStatus;
            AnimeStatus = Init.AnimeStatus;
            ChaptersCount = Init.ChaptersCount;
            EpisodesCount = Init.EpisodesCount;
            AnimeMeanScore = Init.AnimeMeanScore;
            MangaMeanScore = Init.MangaMeanScore;
            ReadedMoreThanOnce = Init.ReadedMoreThanOnce;
            WachedMoreThanOnce = Init.WachedMoreThanOnce;
        }

        // IListStats
        public IReadSpeed ReadSpeed { get; }
        public ulong? ChaptersCount { get; }
        public ulong? EpisodesCount { get; }
        public IDedicatedTime MangaTime { get; }
        public IDedicatedTime AnimeTime { get; }
        public ulong? ReadedMoreThanOnce { get; }
        public ulong? WachedMoreThanOnce { get; }
        public IMeanScore AnimeMeanScore { get; }
        public IMeanScore MangaMeanScore { get; }
        public ISeriesStatus AnimeStatus { get; }
        public ISeriesStatus MangaStatus { get; }
    }
}
