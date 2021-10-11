using System.Collections.Generic;

namespace Shinden.API
{
    public class GetAllMangaCharacters : QueryGet<List<ulong>>
    {
        // Query
        public override string QueryUri => $"{BaseUri}character/in-manga";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}
