using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class RemoveFromFavouriteQuery : QueryDelete<Modification>
    {
        public RemoveFromFavouriteQuery(Models.FavouriteConfig config)
        {
            Config = config;
        }

        private Models.FavouriteConfig Config { get; }

        // Query
        public override string QueryUri => $"{BaseUri}userlist/{Config.UserId}/fav";
        public override string Uri => $"{QueryUri}?api_key={Token}";
        public override HttpContent Content => new StringContent(Config.Build(), Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
