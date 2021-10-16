using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Text;
using System.Security.Claims;
using Sanakan.Config;
using Sanakan.Extensions;
using Sanakan.Api.Models;
using Microsoft.AspNetCore.Http;
using static Sanakan.Web.ResponseExtensions;
using Microsoft.Extensions.Options;
using Sanakan.Web.Configuration;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : ControllerBase
    {
        private readonly SanakanConfiguration _config;
        private readonly IJwtBuilder _jwtBuilder;

        public TokenController(
            IOptions<SanakanConfiguration> config,
            IJwtBuilder jwtBuilder)
        {
            _config = config.Value;
            _jwtBuilder = jwtBuilder;
        }

        /// <summary>
        /// Returns json web token.
        /// </summary>
        /// <param name="apikey">API keyi</param>
        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(TokenData), StatusCodes.Status200OK)]
        public IActionResult CreateToken([FromBody]string apikey)
        {
            if (apikey == null)
            {
                return ShindenUnauthorized("API Key Not Provided");
            }

            var user = _config.ApiKeys
                .FirstOrDefault(x => x.Key.Equals(apikey))?.Bearer;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Website, user),
                new Claim(RegisteredClaimNames.Player, "waifu_player"),
            };

            if (user != null)
            {
                // expires: DateTime.Now.AddHours(24),
                var tokenData = _jwtBuilder.Build(claims);
                return Ok(tokenData);
            }

            return ShindenForbidden(Strings.ApiKeyInvalid);
        }
    }
}
