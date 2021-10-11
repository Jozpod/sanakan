using System.Collections.Generic;

namespace Shinden.API
{
    public class SearchStaffQuery : QueryGet<List<StaffSearchResult>>
    {
        public SearchStaffQuery(string name)
        {
            Name = name;
        }

        private string Name { get; }

        // Query
        public override string QueryUri => $"{BaseUri}staff/search";
        public override string Uri => $"{QueryUri}?query={Name}";
    }
}
