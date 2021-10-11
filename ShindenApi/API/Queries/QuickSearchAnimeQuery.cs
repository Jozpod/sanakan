using System.Collections.Generic;

namespace Shinden.API
{
    public class QuickSearchAnimeQuery : QueryGet<List<QuickSearchResult>>
    {
        public QuickSearchAnimeQuery(string title)
        {
            Title = title;
        }

        private string Title { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/search";
        public override string Uri => $"{QueryUri}?accepted_types=Anime&decode=1&query={Title}";
    }
}
