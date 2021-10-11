using Shinden.API;
using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;
using System.Collections.Generic;
using System.Web;

namespace Shinden.Extensions
{
    public static class AnimeMangaInfoExtension
    {
        public static ITitleInfo ToModel(this AnimeMangaInfo info)
        {
            if (info.Title == null) return null;
            if (info.Title.IsAnime()) return info.Title.ToAnimeModel();
            return info.Title.ToMangaModel();
        }

        public static List<ITitleInfo> ToModel(this List<AnimeMangaInfo> iList)
        {
            var list = new List<ITitleInfo>();
            foreach(var info in iList) list.Add(info.ToModel());
            return list;
        }

        public static bool IsAnime(this AnimeMangaInfo.TitleEntry info)
        {
            return info.Type.ToLower().Equals("anime");
        }

        public static string GetTitleType(this AnimeMangaInfo.TitleEntry info)
        {
            return info.IsAnime() ? info.Anime?.AnimeType : info.Type;
        }

        public static IDateTimePrecision TimeStampDateTime(this AnimeMangaInfo.TitleEntry info, 
            double? unixTimeStamp, ulong? precision)
        {
            return new DateTimePrecision(
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp ?? 0), precision);
        }

        public static double? ConvertToRating(this AnimeMangaInfo.TitleEntry info, string count, string sum)
        {
            double.TryParse(sum ?? "0", out var vSum);
            double.TryParse(count ?? "0", out var vCount);
            if (vSum == 0 || vCount == 0) return 0;
            return vSum / vCount;
        }

        public static DateTime GetDate(this AnimeMangaInfo.TitleEntry title)
        {
            if (title?.AddDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(title.AddDate, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static IDescription ToModel(this AnimeMangaInfo.Description desc)
        {
            ulong.TryParse(desc?.TitleId, out var tID);
            ulong.TryParse(desc?.DescriptionId, out var dID);

            return new Description(new InitDescription()
            {
                Id = dID,
                TitleId = tID,
                Language = new Language().Parse((desc?.LangCode ?? "").ToLower()),
                Content = HttpUtility.HtmlDecode(desc?.OtherDescription)?.RemoveBBCode(),
            });
        }

        public static IAlternativeTitle ToModel(this AnimeMangaInfo.TitleOther title)
        {
            ulong.TryParse(title?.TitleId, out var tID);
            ulong.TryParse(title?.TitleOtherId, out var toID);

            return new AlternativeTitle(new InitAlternativeTitle()
            {
                Id = toID,
                TitleId = tID,
                IsAccepted = (title.IsAccepted ?? "0").Equals("1"),
                Language = new Language().Parse((title?.Lang ?? "").ToLower()),
                Content = HttpUtility.HtmlDecode(title?.Title)?.RemoveBBCode(),
                Type = new AlternativeTitleType().Parse((title?.TitleType ?? "").ToLower()),
            });
        }

        public static List<IAlternativeTitle> ToModel(this List<AnimeMangaInfo.TitleOther> list)
        {
            var rlist = new List<IAlternativeTitle>();
            foreach (var item in list) rlist.Add(item.ToModel());
            return rlist;
        }

        public static List<ITagCategory> GetTags(this AnimeMangaInfo.TitleEntry title)
        {
            var entity = title?.Tags?.Entity?.ToModel();
            var source = title?.Tags?.Source?.ToModel();
            var studio = title?.Tags?.Studio?.ToModel();
            var genre = title?.Tags?.Genre?.ToModel();
            var place = title?.Tags?.Place?.ToModel();
            var tag = title?.Tags?.Tag?.ToModel();

            var list = new List<ITagCategory>();
            if (entity != null) list.Add(entity);
            if (source != null) list.Add(source);
            if (studio != null) list.Add(studio);
            if (genre != null) list.Add(genre);
            if (place != null) list.Add(place);
            if (tag != null) list.Add(tag);

            return list;
        }

        public static ITag ToModel(this AnimeMangaInfo.Item tag)
        {
            ulong.TryParse(tag?.TagId, out var tID);
            ulong.TryParse(tag?.ParentId, out var pID);

            return new Tag(new InitTag()
            {
                Id = tID,
                ParentId = pID,
                Name = tag.TagName,
                NationalName = tag.NationalName,
                IsAccepted = (tag.IsAccepted ?? "0").Equals("1"),

            });
        }

        public static List<ITag> ToModel(this List<AnimeMangaInfo.Item> list)
        {
            var rlist = new List<ITag>();
            foreach (var item in list) rlist.Add(item.ToModel());
            return rlist;
        }

        public static ITagCategory ToModel(this AnimeMangaInfo.Entity tagc)
        {
            return new TagCategory(tagc.Name, new List<AnimeMangaInfo.Item>(tagc?.Items)?.ToModel());
        }

        public static IAnimeTitleInfo ToAnimeModel(this AnimeMangaInfo.TitleEntry title)
        {
            ulong.TryParse(title?.TitleId, out var tID);
            ulong.TryParse(title?.CoverArtifactId, out var cID);
            ulong.TryParse(title?.Anime?.Episodes, out var eCnt);
            ulong.TryParse(title?.RankingPosition, out var rPos);
            ulong.TryParse(title?.Anime?.EpisodeTime, out var eMin);
            ulong.TryParse(title?.FinishPrecision, out var finishPrec);
            ulong.TryParse(title?.PremierePrecision, out var startPrec);

            double.TryParse(title?.FinishDate, out var finishDate);
            double.TryParse(title?.PremiereDate, out var startDate);
            double.TryParse(title?.RankingRate?.Replace('.', ','), out var rankRate);

            return new AnimeTitleInfo(new InitAnimeTitleInfo()
            {
                Id = tID,
                CoverId = cID,
                EpisodesCount = eCnt,
                RankingPosition = rPos,
                RankingRating = rankRate,
                AddDate = title.GetDate(),
                TagCategories = title.GetTags(),
                DMCA = (title.Dmca ?? "0").Equals("1"),
                EpisodeTime = TimeSpan.FromMinutes(eMin),
                Description = title?.Description?.ToModel(),
                Title = HttpUtility.HtmlDecode(title?.OtherTitle),
                StartDate = title.TimeStampDateTime(startDate, startPrec),
                FinishDate = title.TimeStampDateTime(finishDate, finishPrec),
                MPAA = new MpaaRating().Parse((title?.MpaaRating ?? "").ToLower()),
                Type = new AnimeType().Parse((title.GetTitleType() ?? "").ToLower()),
                Status = new AnimeStatus().Parse((title?.TitleStatus ?? "").ToLower()),
                TotalRating = title.ConvertToRating(title?.RatingTotalCnt, title?.RatingTotalSum),
                StoryRating = title.ConvertToRating(title?.RatingStoryCnt, title?.RatingStorySum),
                AlternativeTitles = new List<AnimeMangaInfo.TitleOther>(title?.TitleOther)?.ToModel(),
                MusicRating = title.ConvertToRating(title?.Anime?.RatingMusicCnt, title?.Anime?.RatingMusicSum),
                GraphicRating = title.ConvertToRating(title?.Anime?.RatingGraphicsCnt, title?.Anime?.RatingGraphicsSum),
            });
        }

        public static IMangaTitleInfo ToMangaModel(this AnimeMangaInfo.TitleEntry title)
        {
            ulong.TryParse(title?.TitleId, out var tID);
            ulong.TryParse(title?.Manga?.Volumes, out var vCnt);
            ulong.TryParse(title?.CoverArtifactId, out var cID);
            ulong.TryParse(title?.RankingPosition, out var rPos);
            ulong.TryParse(title?.Manga?.Chapters, out var cCnt);
            ulong.TryParse(title?.FinishPrecision, out var finishPrec);
            ulong.TryParse(title?.PremierePrecision, out var startPrec);

            double.TryParse(title?.FinishDate, out var finishDate);
            double.TryParse(title?.PremiereDate, out var startDate);
            double.TryParse(title?.RankingRate?.Replace('.', ','), out var rankRate);

            return new MangaTitleInfo(new InitMangaTitleInfo()
            {
                Id = tID,
                CoverId = cID,
                VolumesCount = vCnt,
                ChaptersCount = cCnt,
                RankingPosition = rPos,
                RankingRating = rankRate,
                AddDate = title.GetDate(),
                TagCategories = title.GetTags(),
                DMCA = (title.Dmca ?? "0").Equals("1"),
                Description = title?.Description?.ToModel(),
                Title = HttpUtility.HtmlDecode(title?.OtherTitle),
                StartDate = title.TimeStampDateTime(startDate, startPrec),
                FinishDate = title.TimeStampDateTime(finishDate, finishPrec),
                MPAA = new MpaaRating().Parse((title?.MpaaRating ?? "").ToLower()),
                Type = new MangaType().Parse((title.GetTitleType() ?? "").ToLower()),
                Status = new MangaStatus().Parse((title?.TitleStatus ?? "").ToLower()),
                TotalRating = title.ConvertToRating(title?.RatingTotalCnt, title?.RatingTotalSum),
                StoryRating = title.ConvertToRating(title?.RatingStoryCnt, title?.RatingStorySum),
                AlternativeTitles = new List<AnimeMangaInfo.TitleOther>(title?.TitleOther)?.ToModel(),
                LinesRating = title.ConvertToRating(title?.Manga?.RatingLinesCnt, title?.Manga?.RatingLinesSum),
            });
        }
    }
}
