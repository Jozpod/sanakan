namespace Shinden.API
{
    public class GetTitleRecommendationsQuery : QueryGet<TitleRecommendation>
    {
        public GetTitleRecommendationsQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/{Id}/recommendations";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
