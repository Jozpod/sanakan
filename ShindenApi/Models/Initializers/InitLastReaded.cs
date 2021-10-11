using System;

namespace Shinden.Models.Entities
{
    public class InitLastReaded
    {
        public uint ViewCnt { get; set; }
        public ulong TitleId { get; set; }
        public long VolumeNo { get; set; }
        public MangaType Type { get; set; }
        public long ChapterNo { get; set; }
        public ulong ChapterId { get; set; }
        public long VolumesCnt { get; set; }
        public long ChaptersCnt { get; set; }
        public string MangaTitle { get; set; }
        public string ChapterTitle { get; set; }
        public DateTime ReadedDate { get; set; }
        public ulong? TitleCoverId { get; set; }
    }
}
