using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sanakan.Web
{
    public class UserContext : IUserContext
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
                var discordStr = _user.Claims.FirstOrDefault(x => x.Type == "DiscordId")?.Value;

                if (discordStr == null || !ulong.TryParse(discordStr, out var discordId))
                {
                    return null;
                }

                return discordId;
            }
        }
    }
}
