using System;
using System.Collections.Generic;
using Sanakan.ShindenApi.Utilities;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class AnimeTitleInfo : IAnimeTitleInfo
    {
        public AnimeTitleInfo(InitAnimeTitleInfo Init)
        {
            Id = Init.Id;
            Type = Init.Type;
            MPAA = Init.MPAA;
            DMCA = Init.DMCA;
            Title = Init.Title;
            Status = Init.Status;
            CoverId = Init.CoverId;
            AddDate = Init.AddDate;
            StartDate = Init.StartDate;
            FinishDate = Init.FinishDate;
            StoryRating = Init.StoryRating;
            TotalRating = Init.TotalRating;
            MusicRating = Init.MusicRating;
            EpisodeTime = Init.EpisodeTime;
            Description = Init.Description;
            EpisodesCount = Init.EpisodesCount;
            GraphicRating = Init.GraphicRating;
            RankingRating = Init.RankingRating;
            TagCategories = Init.TagCategories;
            RankingPosition = Init.RankingPosition;
            AlternativeTitles = Init.AlternativeTitles;
        }

        public bool? DMCA { get; }
        public ulong? CoverId { get; }

        // IIndexable
        public ulong Id { get; }

        // ISimpleTitleInfo
        public string Title { get; }
        public string CoverUrl => UrlHelpers.GetBigImageURL(CoverId);

        // IAnimeTitleInfo
        public AnimeType Type { get; }
        public MpaaRating MPAA { get; }
        public DateTime AddDate { get; }
        public AnimeStatus Status { get; }
        public double? StoryRating { get; }
        public double? TotalRating { get; }
        public double? MusicRating { get; }
        public TimeSpan EpisodeTime { get; }
        public ulong? EpisodesCount { get; }
        public double? GraphicRating { get; }
        public double? RankingRating { get; }
        public ulong? RankingPosition { get; }
        public IDescription Description { get; }
        public IDateTimePrecision StartDate { get; }
        public IDateTimePrecision FinishDate { get; }
        public List<ITagCategory> TagCategories { get; }
        public List<IAlternativeTitle> AlternativeTitles { get; }

        public string AnimeUrl => UrlHelpers.GetSeriesURL(Id);

        public override string ToString() => Title;
    }
}
