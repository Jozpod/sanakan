using Shinden.API;
using Shinden.Models.Initializers;
using System;

namespace Shinden.Models.Entities
{
    public class NewEpisode : INewEpisode
    {
        public NewEpisode(InitNewEpisode Init)
        {
            Id = Init.EpisodeId;
            AnimeId = Init.AnimeId;
            AddDate = Init.AddDate;
            CoverId = Init.CoverId;
            AnimeTitle = Init.AnimeTitle;
            EpisodeNumber = Init.EpisodeNumber;
            EpisodeLength = Init.EpisodeLength;
            SubtitlesLanguage = Init.SubtitlesLanguage;
        }

        // IIndexable
        public ulong Id { get; }

        public ulong AnimeId { get; }
        public ulong? CoverId { get; }

        // INewEpisode
        public DateTime AddDate { get; }
        public string AnimeTitle { get; }
        public ulong EpisodeNumber { get; }
        public TimeSpan EpisodeLength { get; }
        public Language SubtitlesLanguage { get; }

        public string AnimeUrl => Url.GetSeriesURL(AnimeId);
        public string AnimeCoverUrl => Url.GetBigImageURL(CoverId);
        public string EpisodeUrl => Url.GetEpisodeURL(AnimeId, Id);

        public override string ToString() => $"{AnimeTitle} ({EpisodeNumber})";
    }
}
