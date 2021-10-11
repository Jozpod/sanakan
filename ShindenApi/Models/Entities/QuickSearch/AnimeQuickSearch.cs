using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class AnimeQuickSearch : IAnimeQuickSearch
    {
        public AnimeQuickSearch(InitAnimeQuickSearch Init)
        {
            Type = Init.Type;
            Title = Init.Title;
            Status = Init.Status;
            AnimeId = Init.AnimeId;
            CoverId = Init.CoverId;
        }

        // IIndexable
        public ulong Id => AnimeId;

        public ulong AnimeId { get; }
        public ulong? CoverId { get; }

        // IAnimeQuickSearch
        public string Title { get; }
        public AnimeType Type { get; }
        public AnimeStatus Status { get; }

        public string AnimeUrl => Url.GetSeriesURL(AnimeId);
        public string CoverUrl => Url.GetBigImageURL(CoverId);

        public override string ToString() => Title;
    }
}
