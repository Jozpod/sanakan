namespace Shinden.API
{
    public class GetCharacterfInfoQuery : QueryGet<CharacterInfo>
    {
        public GetCharacterfInfoQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}character/{Id}";
        public override string Uri => $"{QueryUri}?api_key={Token}&fav_stats=1&relations=1&points=1&bio=1&pictures=1";
    }
}
