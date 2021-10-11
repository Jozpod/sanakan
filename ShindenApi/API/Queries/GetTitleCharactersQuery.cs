namespace Shinden.API
{
    public class GetTitleCharactersQuery : QueryGet<TitleCharacters>
    {
        public GetTitleCharactersQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/{Id}/characters";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
