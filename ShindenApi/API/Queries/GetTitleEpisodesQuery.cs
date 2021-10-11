namespace Shinden.API
{
    public class GetTitleEpisodesQuery : QueryGet<TitleEpisodes>
    {
        public GetTitleEpisodesQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/{Id}/episodes";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
