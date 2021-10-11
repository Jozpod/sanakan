namespace Shinden.Models
{
    public interface IListStats
    {
        IReadSpeed ReadSpeed { get;}
        ulong? ChaptersCount { get; }
        ulong? EpisodesCount { get; }
        IDedicatedTime MangaTime { get; }
        IDedicatedTime AnimeTime { get; }
        ulong? ReadedMoreThanOnce { get; }
        ulong? WachedMoreThanOnce { get; }
        IMeanScore AnimeMeanScore { get; }
        IMeanScore MangaMeanScore { get; }
        ISeriesStatus AnimeStatus { get; }
        ISeriesStatus MangaStatus { get; }
    }
}