using System;
using Shinden.API;

namespace Shinden.Models.Entities
{
    public class LastReaded : ILastReaded
    {
        public LastReaded(InitLastReaded Init)
        {
            Type = Init.Type;
            ViewCnt = Init.ViewCnt;
            TitleId = Init.TitleId;
            VolumeNo = Init.VolumeNo;
            ChapterNo = Init.ChapterNo;
            ChapterId = Init.ChapterId;
            VolumesCnt = Init.VolumesCnt;
            MangaTitle = Init.MangaTitle;
            ReadedDate = Init.ReadedDate;
            ChaptersCnt = Init.ChaptersCnt;
            ChapterTitle = Init.ChapterTitle;
            TitleCoverId = Init.TitleCoverId;
        }
        
        private ulong? TitleCoverId { get; }

        // ILastReaded
        public uint ViewCnt { get; }
        public ulong TitleId { get; }
        public long VolumeNo { get; }
        public MangaType Type { get; }
        public long ChapterNo { get; }
        public ulong ChapterId { get; }
        public long VolumesCnt { get; }
        public long ChaptersCnt { get; }
        public string MangaTitle { get; }
        public string ChapterTitle { get; }
        public DateTime ReadedDate { get; }

        public string MangaUrl => Url.GetSeriesURL(TitleId);
        public string MangaCoverUrl => Url.GetBigImageURL(TitleCoverId);
        public string ChapterUrl => Url.GetChapterURL(TitleId, ChapterId);

        public override string ToString() => $"{MangaTitle}: {ChapterNo}/{ChaptersCnt}";
    }
}
