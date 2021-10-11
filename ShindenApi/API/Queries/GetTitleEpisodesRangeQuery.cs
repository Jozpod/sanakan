namespace Shinden.API
{
    public class GetTitleEpisodesRangeQuery : QueryGet<EpisodesRange>
    {
        public GetTitleEpisodesRangeQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}episode/{Id}/range";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
