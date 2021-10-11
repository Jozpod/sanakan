using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class ChangeTitleStatusQuery : QueryPost<TitleStatusAfterChange>
    {
        public ChangeTitleStatusQuery(Models.ChangeTitleStatusConfig config)
        {
            Config = config;
        }

        private Models.ChangeTitleStatusConfig Config { get; }

        // Query
        public override string QueryUri => $"{BaseUri}userlist/{Config.UserId}/series/{Config.TitleId}";
        public override string Uri => $"{QueryUri}?api_key={Token}";
        public override HttpContent Content => new StringContent(Config.Build(), Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
