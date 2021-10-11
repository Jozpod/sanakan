namespace Shinden.Models
{
    public interface IAnimeQuickSearch : IQuickSearch
    {
         AnimeType Type { get; }
         string AnimeUrl { get; }
         AnimeStatus Status { get; }
    }
}