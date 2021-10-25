using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Sanakan.Api.Models;
using Microsoft.AspNetCore.Http;
using static Sanakan.Web.ResponseExtensions;
using Microsoft.Extensions.Options;
using Sanakan.Web.Configuration;
using Sanakan.Web.Resources;
using Sanakan.Configuration;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    {
        private readonly IOptionsMonitor<SanakanConfiguration> _config;
        private readonly IJwtBuilder _jwtBuilder;

        public TokenController(
            IOptionsMonitor<SanakanConfiguration> config,
            IJwtBuilder jwtBuilder)
        {
            _config = config;
            _jwtBuilder = jwtBuilder;
        }

        /// <summary>
        /// Returns json web token.
        /// </summary>
        /// <param name="apikey">API key</param>
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(TokenData), StatusCodes.Status200OK)]
        public IActionResult CreateToken([FromBody]string apikey)
        {
            if (apikey == null)
            {
                return ShindenUnauthorized(Strings.ApiKeyNotProvided);
            }

            var user = _config.CurrentValue.ApiKeys
                .FirstOrDefault(x => x.Key.Equals(apikey))?.Bearer;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Website, user),
                new Claim(RegisteredClaimNames.Player, "waifu_player"),
            };

            if (user != null)
            {
                var tokenData = _jwtBuilder.Build(_config.CurrentValue.TokenExpiry, claims);
                return Ok(tokenData);
            }

            return ShindenForbidden(Strings.ApiKeyInvalid);
        }
    }
}
