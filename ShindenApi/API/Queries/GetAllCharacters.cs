using System.Collections.Generic;

namespace Shinden.API
{
    public class GetAllCharacters : QueryGet<List<ulong>>
    {
        // Query
        public override string QueryUri => $"{BaseUri}character/connected";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
