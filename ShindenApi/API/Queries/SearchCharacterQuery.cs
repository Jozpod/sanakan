using System.Collections.Generic;

namespace Shinden.API
{
    public class SearchCharacterQuery : QueryGet<List<CharacterSearchResult>>
    {
        public SearchCharacterQuery(string name)
        {
            Name = name;
        }

        private string Name { get; }

        // Query
        public override string QueryUri => $"{BaseUri}character/search";
        public override string Uri => $"{QueryUri}?query={Name}";
    }
}
