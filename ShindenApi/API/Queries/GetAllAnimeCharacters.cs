using System.Collections.Generic;

namespace Shinden.API
{
    public class GetAllAnimeCharacters : QueryGet<List<ulong>>
    {
        // Query
        public override string QueryUri => $"{BaseUri}character/in-anime";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
