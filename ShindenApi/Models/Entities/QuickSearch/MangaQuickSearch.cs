using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class MangaQuickSearch : IMangaQuickSearch
    {
        public MangaQuickSearch(InitMangaQuickSearch Init)
        {
            Type = Init.Type;
            Title = Init.Title;
            Status = Init.Status;
            MangaId = Init.MangaId;
            CoverId = Init.CoverId;
        }

        // IIndexable
        public ulong Id => MangaId;

        public ulong MangaId { get; }
        public ulong? CoverId { get; }

        // IMangaQuickSearch
        public string Title { get; }
        public MangaType Type { get; }
        public MangaStatus Status { get; }

        public string MangaUrl => Url.GetMangaURL(MangaId);
        public string CoverUrl => Url.GetBigImageURL(CoverId);

        public override string ToString() => Title;
    }
}
