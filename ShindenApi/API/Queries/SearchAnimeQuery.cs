using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class SearchAnimeQuery : QueryPost<SearchResult>
    {
        public SearchAnimeQuery(AnimeSearchConfig config)
        {
            Config = config;
        }

        private AnimeSearchConfig Config { get; }

        // Query
        public override string QueryUri => $"{BaseUri}anime/search";
        public override string Uri => $"{QueryUri}?api_key={Token}&lang=pl&decode=1&for_season=1";
        public override HttpContent Content => new StringContent(Config.Build(), Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
