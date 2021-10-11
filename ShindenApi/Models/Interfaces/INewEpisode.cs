using System;

namespace Shinden.Models
{
    public interface INewEpisode : IIndexable
    {
        string AnimeUrl { get; }
        DateTime AddDate { get; }
        string AnimeTitle { get; }
        string EpisodeUrl { get; }
        ulong EpisodeNumber { get; }
        string AnimeCoverUrl { get; }
        TimeSpan EpisodeLength { get; }
        Language SubtitlesLanguage { get; }
    }
}
