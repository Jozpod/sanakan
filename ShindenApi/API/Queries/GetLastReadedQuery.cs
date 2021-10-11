using System.Collections.Generic;

namespace Shinden.API
{
    public class GetLastReadedQuery : QueryGet<List<LastWatchedReaded>>
    {
        public GetLastReadedQuery(ulong userId, uint limit = 5)
        {
            Id = userId;
            Limit = limit;
        }

        private ulong Id { get; }
        private uint Limit { get; }

        // Query
        public override string QueryUri => $"{BaseUri}user/{Id}/last_read";
        public override string Uri => $"{QueryUri}?api_key={Token}&limit={Limit}";
    }
}
