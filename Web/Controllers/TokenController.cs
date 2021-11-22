using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sanakan.Api.Models;
using Sanakan.Common.Configuration;
using Sanakan.Web.Resources;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using static Sanakan.Web.ResponseExtensions;

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
        /// Creates json web token with website claim.
        /// </summary>
        /// <param name="apikey">API key.</param>
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(TokenData), StatusCodes.Status200OK)]
        public IActionResult CreateToken([FromBody]string? apikey = null)
        {
            if (string.IsNullOrEmpty(apikey))
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
                };

            var tokenData = _jwtBuilder.Build(_config.CurrentValue.Jwt.TokenExpiry, claims);
            return Ok(tokenData);
        }
    }
}
