namespace Shinden.Models
{
    public interface IMangaTitleInfo : ITitleInfo
    {
        MangaType Type { get; }
        string MangaUrl { get; }
        MangaStatus Status { get; }
        ulong? VolumesCount { get; }
        double? LinesRating { get; }
        ulong? ChaptersCount { get; }
    }
}
