using System;
using Shinden.Models;

namespace Shinden
{
    public enum TagIncludeType
    {
        All,
        One,
    }

    public enum SearchTitleType
    {
        EQUALS,
        CONTAINS,
    }

    public enum LengthInterval
    {
        Less_7,
        From_7_To_18,
        From_19_To_27,
        From_28_To_48,
        Over_48,
    }

    public enum CountInterval
    {
        Only_1,
        From_2_To_14,
        From_15_To_28,
        From_29_To_100,
        Over_100,
    }

    public enum DatePrecision
    {
        Year,
        Year_Month,
        Year_Month_Day,
    }

    public enum SortType
    {
        Type,
        Rand,
        Score,
        Status,
        Popular,
        Multimedia,
        Ranking_Rate,
    }
    
    public enum SortOrderType
    {
        ASC,
        DESC,
    }

    public class AnimeSearchConfig
    {
        public uint? PerPage { get; set; }
        public DateTime? DateTo { get; set; }
        public SortType? SortBy { get; set; }
        public TagsStatuses Tags { get; set; }
        public char? FirstLetter { get; set; }
        public DateTime? DateFrom { get; set; }
        public AnimeType?[] Types { get; set; }
        public string SearchString { get; set; }
        public bool? PlayersAvailable { get; set; }
        public AnimeStatus?[] Statuses { get; set; }
        public SortOrderType? SortOrder { get; set; }
        public TagIncludeType? TagInclude { get; set; }
        public SearchTitleType? SearchType { get; set; }
        public DatePrecision? DatePrecision { get; set; }
        public CountInterval?[] EpisodesCount { get; set; }
        public LengthInterval?[] EpisodeLength { get; set; }
        public EpisodesCount CustomEpisodesCount { get; set; }
    }

    public class EpisodesCount
    {
        public uint? From { get; set; }
        public uint? To { get; set; }
    }

    public class TagsStatuses
    {
        public uint?[] Include { get; set; }
        public uint?[] Exclude { get; set; }
    }
}