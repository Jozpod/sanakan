namespace Shinden.API
{
    public class GetTitleReviewsQuery : QueryGet<TitleReviews>
    {
        public GetTitleReviewsQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/{Id}/reviews";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
