using System.Collections.Generic;

namespace Shinden.API
{
    public class GetUserFavCharactersQuery : QueryGet<List<FavCharacter>>
    {
        public GetUserFavCharactersQuery(ulong id)
        {
            Id = id;
        }

        private ulong Id { get; }

        // Query
        public override string QueryUri => $"{BaseUri}user/{Id}/fav-chars";
        public override string Uri => $"{QueryUri}?api_key={Token}";
    }
}