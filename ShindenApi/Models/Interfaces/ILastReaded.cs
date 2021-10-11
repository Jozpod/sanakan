using System;

namespace Shinden.Models
{
    public interface ILastReaded
    {
        uint ViewCnt { get; }
        ulong TitleId { get; }
        long VolumeNo { get; }
        MangaType Type { get; }
        long ChapterNo { get; }
        string MangaUrl { get; }
        ulong ChapterId { get; }
        long VolumesCnt { get; }
        long ChaptersCnt { get; }
        string ChapterUrl { get; }
        string MangaTitle { get; }
        string ChapterTitle { get; }
        DateTime ReadedDate { get; }
        string MangaCoverUrl { get; }
    }
}