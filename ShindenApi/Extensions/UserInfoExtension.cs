using Shinden.Models;
using Shinden.Models.Entities;
using Shinden.Models.Initializers;
using System;

namespace Shinden.Extensions
{
    public static class UserInfoExtension
    {
        public static DateTime GetRegisterDate(this API.UserInfo info)
        {
            if (info?.RegisterDate == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.RegisterDate, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime GetLastActiveDate(this API.UserInfo info)
        {
            if (info?.LastActive == null) return DateTime.MinValue;

            return DateTime.ParseExact(info.LastActive, "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        public static IDedicatedTime ToModel(this API.Time time)
        {
            ulong.TryParse(time.Y, out var year);
            ulong.TryParse(time.D, out var days);
            ulong.TryParse(time.H, out var hours);
            ulong.TryParse(time.N, out var months);
            ulong.TryParse(time.M, out var minutes);

            return new DedicatedTime(new InitDedicatedTime()
            {
                Days = days,
                Years = year,
                Hours = hours,
                Months = months,
                Minutes = minutes,
            });
        }

        public static ISeriesStatus ToModel(this API.ReadWatchStatuses rwStats)
        {
            ulong.TryParse(rwStats?.Total, out var all);
            ulong.TryParse(rwStats?.Skip, out var skip);
            ulong.TryParse(rwStats?.Hold, out var hold);
            ulong.TryParse(rwStats?.Plan, out var plan);
            ulong.TryParse(rwStats?.Dropped, out var dropped);
            ulong.TryParse(rwStats?.Completed, out var completed);
            ulong.TryParse(rwStats?.InProgress, out var progress);

            return new SeriesStatus(new InitSeriesStatus()
            {
                InPlan = plan,
                OnHold = hold,
                Skipped = skip,
                Dropped = dropped,
                Completed = completed,
                InProgress = progress,
            });
        }

        public static IListStats ToListStatsModel(this API.UserInfo info)
        {
            ulong.TryParse(info?.ReadedCnt, out var chapters);
            ulong.TryParse(info?.WatchedCnt, out var episodes);
            ulong.TryParse(info?.ReadMoreThan1, out var readMore);
            double.TryParse(info?.ReadSpeed?.VnProc, out var vnProc);
            ulong.TryParse(info?.WatchedMoreThan1, out var watchedMore);
            double.TryParse(info?.ReadSpeed?.MangaProc, out var mangProc);
            ulong.TryParse(info?.ReadSpeed?.MangaReadTime, out var readTime);
            ulong.TryParse(info?.MeanAnimeScore?.ScoresCnt, out var animeScoreCount);
            ulong.TryParse(info?.MeanMangaScore?.ScoresCnt, out var mangaScoreCount);
            double.TryParse(info?.MeanAnimeScore?.OtherMeanScore?.Replace('.', ','), out var animeRating);
            double.TryParse(info?.MeanMangaScore?.OtherMeanScore?.Replace('.', ','), out var mangaRating);

            return new ListStats(new InitListStats()
            {
                ChaptersCount = chapters,
                EpisodesCount = episodes,
                ReadedMoreThanOnce = readMore,
                WachedMoreThanOnce = watchedMore,
                MangaTime = info?.ReadTime?.ToModel(),
                AnimeTime = info?.WatchTime?.ToModel(),
                MangaStatus = info?.ReadedStatus?.ToModel(),
                AnimeStatus = info?.WatchedStatus?.ToModel(),
                ReadSpeed = new ReadSpeed(readTime, mangProc, vnProc),
                AnimeMeanScore = new MeanScore(animeScoreCount, animeRating),
                MangaMeanScore = new MeanScore(mangaScoreCount, mangaRating),
            });
        }

        public static IUserInfo ToModel(this API.UserInfo info)
        {
            ulong.TryParse(info?.VbId, out var vID);
            ulong.TryParse(info?.UserId, out var uID);
            ulong.TryParse(info?.Avatar, out var avatar);
            ulong.TryParse(info?.SkinId, out var skinID);
            ulong.TryParse(info?.LogoImgId, out var logoID);
            long.TryParse(info?.TotalPoints, out var points);

            return new UserInfo(new InitUserInfo()
            {
                Id = uID,
                ForumId = vID,
                SkinId = skinID,
                Name = info?.Name,
                Rank = info?.Rank,
                AvatarId = avatar,
                LogoImgId = logoID,
                Email = info?.Email,
                TotalPoints = points,
                AboutMe = info?.AboutMe,
                AnimeCSS = info?.AnimeCss,
                MangaCSS = info?.MangaCss,
                Signature = info?.Signature,
                ListStats = info?.ToListStatsModel(),
                RegisterDate = info.GetRegisterDate(),
                LastTimeActive = info.GetLastActiveDate(),
                Status = new UserStatus().Parse((info?.Status ?? "").ToLower()),
                PortalLanguage = new Language().Parse((info?.PortalLang ?? "").ToLower()),
            });
        }
    }
}
