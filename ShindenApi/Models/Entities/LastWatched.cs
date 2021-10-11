using System;
using Shinden.API;

namespace Shinden.Models.Entities
{
    public class LastWatched : ILastWatched
    {
        public LastWatched(InitLastWatched Init)
        {
            Type = Init.Type;
            ViewCnt = Init.ViewCnt;
            IsFiler = Init.IsFiler;
            TitleId = Init.TitleId;
            IsSpecial = Init.IsSpecial;
            EpisodeNo = Init.EpisodeNo;
            EpisodeId = Init.EpisodeId;
            AnimeTitle = Init.AnimeTitle;
            EpisodesCnt = Init.EpisodesCnt;
            WatchedDate = Init.WatchedDate;
            TitleCoverId = Init.TitleCoverId;
            EpisodeTitle = Init.EpisodeTitle;
        }
        
        private ulong? TitleCoverId { get; }

        // ILastWatched
        public uint ViewCnt { get; }
        public bool IsFiler { get; }
        public ulong TitleId { get; }
        public bool IsSpecial { get; }
        public long EpisodeNo { get; }
        public AnimeType Type { get; }
        public ulong EpisodeId { get; }
        public long EpisodesCnt { get; }
        public string AnimeTitle { get; }
        public string EpisodeTitle { get; }
        public DateTime WatchedDate { get; }

        public string AnimeUrl => Url.GetSeriesURL(TitleId);
        public string AnimeCoverUrl => Url.GetBigImageURL(TitleCoverId);
        public string EpisodeUrl => Url.GetEpisodeURL(TitleId, EpisodeId);

        public override string ToString() => $"{AnimeTitle}: {EpisodeNo}/{EpisodesCnt}";
    }
}
