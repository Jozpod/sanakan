using System;
using System.Collections.Generic;
using Shinden.Models;
using Shinden.Models.Entities;

namespace Shinden.Extensions
{
    public static class LastWatchedReadedExtension
    {
        public static ILastWatched ToAnimeModel(this API.LastWatchedReaded anime)
        {
            ulong.TryParse(anime?.CoverArtifactId, out var cID);
            ulong.TryParse(anime?.EpisodeId, out var eID);
            ulong.TryParse(anime?.TitleId, out var tID);

            long.TryParse(anime?.EpisodeNo, out var eNO);
            long.TryParse(anime?.Episodes, out var eCNT);

            uint.TryParse(anime?.ViewCnt, out var vCNT);

            bool.TryParse(anime?.IsFiler, out var filter);
            bool.TryParse(anime?.IsSpecial, out var special);

            return new LastWatched(new InitLastWatched()
            {
                TitleId = tID,
                ViewCnt = vCNT,
                EpisodeId = eID,
                EpisodeNo = eNO,
                IsFiler = filter,
                EpisodeTitle = "", // TODO
                TitleCoverId = cID,
                EpisodesCnt = eCNT,
                IsSpecial = special,
                AnimeTitle = anime.Title,
                WatchedDate = anime.GetDate(),
                Type = new AnimeType().Parse((anime.AnimeType ?? "").ToLower()),
            });
        }

        public static List<ILastWatched> ToAnimeModel(this List<API.LastWatchedReaded> eList)
        {
            var list = new List<ILastWatched>();
            foreach(var item in eList) list.Add(item.ToAnimeModel());
            return list;
        }

        public static ILastReaded ToMangaModel(this API.LastWatchedReaded manga)
        {
            ulong.TryParse(manga?.CoverArtifactId, out var cID);
            ulong.TryParse(manga?.ChapterId, out var caID);
            ulong.TryParse(manga?.TitleId, out var tID);

            long.TryParse(manga?.ChapterNo, out var cNO);
            long.TryParse(manga?.Chapters, out var cCNT);
            long.TryParse(manga?.VolumeNo, out var vNO);
            long.TryParse(manga?.Volumes, out var voCNT);

            uint.TryParse(manga?.ViewCnt, out var vCNT);

            return new LastReaded(new InitLastReaded()
            {
                TitleId = tID,
                ViewCnt = vCNT,
                VolumeNo = vNO,
                ChapterNo = cNO,
                ChapterId = caID,
                ChapterTitle = "", // TODO
                TitleCoverId = cID,
                ChaptersCnt = cCNT,
                VolumesCnt = voCNT,
                MangaTitle = manga.Title,
                ReadedDate = manga.GetDate(),
                Type = new MangaType().Parse((manga.Type ?? "").ToLower()),
            });
        }

        public static List<ILastReaded> ToMangaModel(this List<API.LastWatchedReaded> eList)
        {
            var list = new List<ILastReaded>();
            foreach(var item in eList) list.Add(item.ToMangaModel());
            return list;
        }

        public static DateTime GetDate(this API.LastWatchedReaded title)
        {
            if (title?.CreatedTime == null) return DateTime.MinValue;

            return DateTime.ParseExact(title.CreatedTime, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
