using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class RateAnimeQuery : QueryPost<Status>
    {
        public RateAnimeQuery(Models.RateAnimeConfig config)
        {
            Config = config;
        }

        private Models.RateAnimeConfig Config { get; }

        // Query
        public override string QueryUri => $"{BaseUri}anime/{Config.TitleId}/rate";
        public override string Uri => $"{QueryUri}?api_key={Token}";
        public override HttpContent Content => new StringContent(Config.Build(), Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
