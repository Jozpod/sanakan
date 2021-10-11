using System;
using System.Collections.Generic;
using Shinden.Models;

namespace Shinden.Extensions
{
    public static class AnimeSearchConfigExtension
    {
        public static string ToValue(this SortOrderType? type)
        {
            return type?.ToString().ToLower();
        }

        public static string ToValue(this SortType? type)
        {
            return type?.ToString().Replace("_", "-").ToLower();
        }

        public static string ToValue(this TagIncludeType? type)
        {
            return type?.ToString().ToLower();
        }

        public static string ToValue(this SearchTitleType? type)
        {
            return type?.ToString().ToLower();
        }

        public static long? ToValue(this DatePrecision? type)
        {
            if (!type.HasValue) return null;
            switch (type.Value)
            {
                case DatePrecision.Year: return 1;
                case DatePrecision.Year_Month: return 2;
                case DatePrecision.Year_Month_Day: return 3;
                default: return 1;
            }
        }

        public static string ToValue(this DateTime? time)
        {
            return time?.ToString("yyyy-MM-dd");
        }

        public static string ToValue(this AnimeType? type)
        {
            if (!type.HasValue) return null;
            switch(type.Value)
            {
                case AnimeType.Tv: return "TV";
                case AnimeType.Ova: return "OVA";
                case AnimeType.Movie: return "Movie";
                case AnimeType.Special: return "Special";
                case AnimeType.Ona: return "ONA";
                case AnimeType.Music: return "Music";
                case AnimeType.NotSpecified:
                default: return null;
            }
        }

        public static string ToValue(this AnimeStatus? type)
        {
            if (!type.HasValue) return null;
            switch(type.Value)
            {
                case AnimeStatus.CurrentlyAiring: return "Currently Airing";
                case AnimeStatus.FinishedAiring: return "Finished Airing";
                case AnimeStatus.NotYetAired: return "Not yet aired";
                case AnimeStatus.Proposal: return "Proposal";
                case AnimeStatus.NotSpecified:
                default: return null;
            }
        }

        public static string ToValue(this LengthInterval? length)
        {
            return !length.HasValue ? null : length.ToString().ToLower().Replace("from_", "");
        }

        public static string ToValue(this CountInterval? length)
        {
            return !length.HasValue ? null : length.ToString().ToLower().Replace("from_", "");
        }

        public static string[] ToValue(this LengthInterval?[] types)
        {
            if (types == null) return null;
            if (types.Length <= 0) return null;
            var list = new List<string>();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i].ToValue();
                if (type != null) list.Add(type);
            }
            return list.ToArray();
        }

        public static string[] ToValue(this CountInterval?[] types)
        {
            if (types == null) return null;
            if (types.Length <= 0) return null;
            var list = new List<string>();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i].ToValue();
                if (type != null) list.Add(type);
            }
            return list.ToArray();
        }

        public static string[] ToValue(this AnimeStatus?[] types)
        {
            if (types == null) return null;
            if (types.Length <= 0) return null;
            var list = new List<string>();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i].ToValue();
                if (type != null) list.Add(type);
            }
            return list.ToArray();
        }

        public static string[] ToValue(this AnimeType?[] types)
        {
            if (types == null) return null;
            if (types.Length <= 0) return null;
            var list = new List<string>();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i].ToValue();
                if (type != null) list.Add(type);
            }
            return list.ToArray();
        }

        public static string ToValue(this TagsStatuses tags)
        {
            if (tags == null) return null;
            if (tags.Include == null && tags.Exclude == null) return null;
            
            var inc = new List<string>();
            if (tags.Include != null)
            {
                for (var i = 0; i < tags.Include.Length; i++)
                {
                    if (tags.Include[i].HasValue)
                    {
                        inc.Add($"{i}" + tags.Include[i].Value);
                    }
                }
            }
            var exc = new List<string>();
            if (tags.Exclude != null)
            {
                for (var i = 0; i < tags.Exclude.Length; i++)
                {
                    if (!tags.Exclude[i].HasValue) continue;

                    var val = tags.Exclude[i].Value.ToString();
                    if (!inc.Contains($"{i}" + val)) inc.Add("e" + val);
                }
            }

            inc.AddRange(exc);
            return string.Join(";", inc);
        }

        public static string Build(this AnimeSearchConfig config, uint offset = 0)
        {
            var list = new List<string>();
            list.AppendPOST("offset", offset);
            list.AppendPOST("for_seasons", 1);
            list.AppendPOST("per_page", config.PerPage);
            list.AppendPOSTAsInt("one_online", config.PlayersAvailable);
            list.AppendPOST("sort_order", config.SortOrder.ToValue());
            list.AppendPOST("sort_by", config.SortBy.ToValue());
            list.AppendPOST("start_date_precision", config.DatePrecision.ToValue());
            list.AppendPOST("genres-type", config.TagInclude.ToValue());
            list.AppendPOST("type", config.SearchType.ToValue());
            list.AppendPOST("letter", config.FirstLetter);
            list.AppendPOST("year_from", config.DateFrom.ToValue());
            list.AppendPOST("year_to", config.DateTo.ToValue());
            list.AppendPOST("series_number_from", config.CustomEpisodesCount?.From);
            list.AppendPOST("series_number_to", config.CustomEpisodesCount?.To);
            list.AppendPOST("series_type", config.Types.ToValue());
            list.AppendPOST("series_status", config.Statuses.ToValue());
            list.AppendPOST("series_length", config.EpisodeLength.ToValue());
            list.AppendPOST("series_number", config.EpisodesCount.ToValue());
            list.AppendPOST("genres", config.Tags?.ToValue());
            list.AppendPOST("search", config.SearchString);
            return string.Join("&", list);
        }
    }
}