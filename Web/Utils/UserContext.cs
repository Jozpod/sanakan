using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Sanakan.Web
{
    internal class UserContext : IUserContext
    {
        private readonly ClaimsPrincipal _user;

        public UserContext(IHttpContextAccessor httpContextAcessor)
        {
            _user = httpContextAcessor.HttpContext.User;
        }

        public ulong? DiscordId
        {
            get
            {
                var discordStr = _user.Claims.FirstOrDefault(x => x.Type == nameof(DiscordId))?.Value;

                if (discordStr == null || !ulong.TryParse(discordStr, out var discordId))
                {
                    return null;
                }

                return discordId;
            }
        }

        public bool HasWebpageClaim() => _user.HasClaim(x => x.Type == ClaimTypes.Webpage);
    }
}
