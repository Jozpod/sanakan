using System;
using System.Collections.Generic;
using System.Web;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;

namespace Shinden.Extensions
{
    public static class EpisodeExtension
    {
        public static List<IEpisode> ToModel(this API.TitleEpisodes episodes)
        {
            var list = new List<IEpisode>();
            foreach(var item in episodes.Episodes) list.Add(item.ToModel());
            foreach(var item in episodes.ConnectedEpisodes) list.Add(item.ToModel());
            return list;
        }

        public static IEpisode ToModel(this API.Episode episode)
        {
            ulong.TryParse(episode?.TitleId, out var tID);
            ulong.TryParse(episode?.EpisodeId, out var eID);
            ulong.TryParse(episode?.EpisodeNo, out var eNo);
            ulong.TryParse(episode?.EpisodeTitleId, out var etID);

            bool.TryParse(episode?.IsAccepted, out var acc);
            bool.TryParse(episode?.HasOnline, out var hos);
            bool.TryParse(episode?.IsFiler, out var fil);

            return new Episode(new InitEpisode()
            {
                Id = eID,
                AnimeId = tID,
                IsFiller = fil,
                HasOnline = hos,
                EpisodeNumber = eNo,
                AirDate = episode.GetDate(),
                EpisodeLength = episode.GetLength(),
                AirChannel = episode?.AirChannell?.RemoveBBCode(),
                Type = new EpisodeType().Parse((episode?.Type ?? "").ToLower()),
                EpisodeTitle = new AlternativeTitle(new InitAlternativeTitle()
                {
                    Id = etID,
                    TitleId = tID,
                    IsAccepted = acc,
                    Language = new Language().Parse((episode?.Lang ?? "").ToLower()),
                    Content = HttpUtility.HtmlDecode(episode?.Title)?.RemoveBBCode(),
                    Type = new AlternativeTitleType().Parse((episode?.TitleType ?? "").ToLower()),
                })
            });
        }

        public static List<IEpisode> ToModel(this List<API.Episode> eList)
        {
            var list = new List<IEpisode>();
            foreach(var item in eList) list.Add(item.ToModel());
            return list;
        }

        public static DateTime GetDate(this API.Episode episode)
        {
            if (episode?.AirDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(episode.AirDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static TimeSpan GetLength(this API.Episode episode)
        {
            ulong.TryParse(episode?.EpisodeTime, out var min);
            return TimeSpan.FromMinutes(min);
        }
    }
}
