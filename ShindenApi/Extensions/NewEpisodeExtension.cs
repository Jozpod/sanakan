using System;
using System.Collections.Generic;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;

namespace Shinden.Extensions
{
    public static class NewEpisodeExtension
    {
        public static NewEpisode ToModel(this API.NewEpisode episode)
        {
            ulong.TryParse(episode?.TitleId, out var tID);
            ulong.TryParse(episode?.EpisodeId, out var eID);
            ulong.TryParse(episode?.EpisodeNo, out var eNo);
            ulong.TryParse(episode?.Cover_artifact_id, out var cID);

            return new NewEpisode(new InitNewEpisode()
            {
                AnimeId = tID,
                CoverId = cID,
                EpisodeId = eID,
                EpisodeNumber = eNo,
                AddDate = episode.GetDate(),
                AnimeTitle = episode?.Title ?? "",
                EpisodeLength = episode.GetLength(),
                SubtitlesLanguage = episode.GetLanguage()
            });
        }

        public static List<NewEpisode> ToModel(this List<API.NewEpisode> eList)
        {
            var list = new List<NewEpisode>();
            foreach(var item in eList) list.Add(item.ToModel());
            return list;
        }

        public static DateTime GetDate(this API.NewEpisode episode)
        {
            if (episode?.AddDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(episode.AddDate, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static TimeSpan GetLength(this API.NewEpisode episode)
        {
            ulong.TryParse(episode?.EpisodeTime, out var min);
            return TimeSpan.FromMinutes(min);
        }

        public static Language GetLanguage(this API.NewEpisode episode)
        {
            return new Language().Parse((episode?.SubLang ?? "").ToLower());
        }
    }
}
