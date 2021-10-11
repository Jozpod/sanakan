using System;
using System.Collections.Generic;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class MangaTitleInfo : IMangaTitleInfo
    {

        public MangaTitleInfo(InitMangaTitleInfo Init)
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
            Description = Init.Description;
            LinesRating = Init.LinesRating;
            VolumesCount = Init.VolumesCount;
            ChaptersCount = Init.ChaptersCount;
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
        public string CoverUrl => Url.GetBigImageURL(CoverId);

        // IMangaTitleInfo
        public MangaType Type { get; }
        public MpaaRating MPAA { get; }
        public DateTime AddDate { get; }
        public MangaStatus Status { get; }
        public double? StoryRating { get; }
        public double? TotalRating { get; }
        public ulong? VolumesCount { get; }
        public double? LinesRating { get; }
        public ulong? ChaptersCount { get; }
        public double? RankingRating { get; }
        public ulong? RankingPosition { get; }
        public IDescription Description { get; }
        public IDateTimePrecision StartDate { get; }
        public IDateTimePrecision FinishDate { get; }
        public List<ITagCategory> TagCategories { get; }
        public List<IAlternativeTitle> AlternativeTitles { get; }

        public string MangaUrl => Url.GetMangaURL(Id);

        public override string ToString() => Title;
    }
}
