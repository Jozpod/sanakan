using System.Collections.Generic;

namespace Shinden.API
{
    public class SearchUserQuery : QueryGet<List<UserSearchResult>>
    {
        public SearchUserQuery(string nick)
        {
            Nick = nick;
        }

        private string Nick { get; }

        // Query
        public override string QueryUri => $"{BaseUri}user/search";
        public override string Uri => $"{QueryUri}?query={Nick}";
    }
}
