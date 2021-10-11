using System.Collections.Generic;

namespace Shinden.API
{
    public class GetLastWatchedQuery : QueryGet<List<LastWatchedReaded>>
    {
        public GetLastWatchedQuery(ulong userId, uint limit = 5)
        {
            Id = userId;
            Limit = limit;
        }

        private ulong Id { get; }
        private uint Limit { get; }

        // Query
        public override string QueryUri => $"{BaseUri}user/{Id}/last_view";
        public override string Uri => $"{QueryUri}?api_key={Token}&limit={Limit}";
    }
}
