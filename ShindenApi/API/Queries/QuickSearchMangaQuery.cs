using System.Collections.Generic;

namespace Shinden.API
{
    public class QuickSearchMangaQuery : QueryGet<List<QuickSearchResult>>
    {
        public QuickSearchMangaQuery(string title)
        {
            Title = title;
        }

        private string Title { get; }

        // Query
        public override string QueryUri => $"{BaseUri}title/search";
        public override string Uri => $"{QueryUri}?accepted_types=Manga%3BManhua%3BNovel%3BDoujin%3BManhwa%3BOEL%3BOne+Shot&decode=1&query={Title}";
    }
}
