namespace Shinden.API
{
    public class GetTitleRelatedQuery : QueryGet<TitleRelations>
    {
        public GetTitleRelatedQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/{Id}/related";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
