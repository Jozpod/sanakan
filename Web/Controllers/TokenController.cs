using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Sanakan.Api.Models;
using Microsoft.AspNetCore.Http;
using static Sanakan.Web.ResponseExtensions;
using Microsoft.Extensions.Options;
using Sanakan.Web.Resources;
using Sanakan.Common.Configuration;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    {
        private readonly IOptionsMonitor<ApiConfiguration> _config;
        private readonly IJwtBuilder _jwtBuilder;

        public TokenController(
            IOptionsMonitor<ApiConfiguration> config,
            IJwtBuilder jwtBuilder)
        {
            _config = config;
            _jwtBuilder = jwtBuilder;
        }

        /// <summary>
        /// Creates json web token with website claim
        /// </summary>
        /// <param name="apikey">API key</param>
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(TokenData), StatusCodes.Status200OK)]
        public IActionResult CreateToken([FromBody]string? apikey = null)
        {
            if (apikey == null)
            {
                return ShindenUnauthorized(Strings.ApiKeyNotProvided);
            }

            var bearer = _config.CurrentValue.ApiKeys
                .FirstOrDefault(x => x.Key.Equals(apikey))?.Bearer;

            if (bearer == null)
            {
                return ShindenForbidden(Strings.ApiKeyInvalid);
                
            }

            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Website, bearer),
                    new Claim(RegisteredClaimNames.Player, "waifu_player"),
                };

            var tokenData = _jwtBuilder.Build(_config.CurrentValue.TokenExpiry, claims);
            return Ok(tokenData);
        }
    }
}
