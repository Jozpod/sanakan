using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class RateMangaQuery : QueryPost<Status>
    {
        public RateMangaQuery(Models.RateMangaConfig config)
        {
            Config = config;
        }

        private Models.RateMangaConfig Config { get; }

        // Query
        public override string QueryUri => $"{BaseUri}manga/{Config.TitleId}/rate";
        public override string Uri => $"{QueryUri}?api_key={Token}";
        public override HttpContent Content => new StringContent(Config.Build(), Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
