namespace Shinden.Models
{
    public interface IMangaQuickSearch : IQuickSearch
    {
         MangaType Type { get; }
         string MangaUrl { get; }
         MangaStatus Status { get; }
    }
}