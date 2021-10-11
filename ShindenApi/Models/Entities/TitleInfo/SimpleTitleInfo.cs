using Shinden.API;

namespace Shinden.Models.Entities
{
    public class SimpleTitleInfo : ISimpleTitleInfo
    {

        public SimpleTitleInfo(ulong Id, string Title, ulong? CoverId)
        {
            this.Id = Id;
            this.Title = Title;
            this.CoverId = CoverId;
        }

        public ulong? CoverId { get; }

        // IIndexable
        public ulong Id { get; }

        // ISimpleTitleInfo
        public string Title { get; }
        public string CoverUrl => Url.GetBigImageURL(CoverId);

        public override string ToString() => Title;
    }
}
