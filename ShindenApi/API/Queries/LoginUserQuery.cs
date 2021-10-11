using System.Net.Http;
using Shinden.Extensions;
using System.Text;

namespace Shinden.API
{
    public class LoginUserQuery : QueryPost<Logging>
    {
        public LoginUserQuery(UserAuth auth)
        {
            Auth = auth;
        }

        private UserAuth Auth { get; }

        // Query
        public override string QueryUri => $"{BaseUri}user/login";
        public override string Uri => $"{QueryUri}?api_key={Token}";
        public override HttpContent Content => new StringContent(Auth.Build(), Encoding.UTF8, "application/x-www-form-urlencoded");
    }
}
