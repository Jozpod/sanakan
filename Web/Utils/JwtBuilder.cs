using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Web.Configuration;

namespace Sanakan.Api
{
    public class JwtBuilder : IJwtBuilder
    {
        private readonly Encoding _encoding;
        private readonly JwtConfig _options;
        private readonly ISystemClock _systemClock;
        private readonly SigningCredentials _signingCredentials;

        public JwtBuilder(
            IOptions<JwtConfig> options,
            Encoding encoding,
            ISystemClock systemClock)
        {
            _options = options.Value;
            _encoding = encoding; // Encoding.UTF8
            var securityKey = new SymmetricSecurityKey(_encoding.GetBytes(_options.Key));
            _systemClock = systemClock;
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        public TokenData Build(params Claim[] claims)
        {
            var allClaims = claims.Append(
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var token = new JwtSecurityToken(_options.Issuer,
              _options.Issuer,
              allClaims,
              expires: _systemClock.UtcNow + _options.ExpiresOn, //DateTime.Now.AddMinutes(30),
              signingCredentials: _signingCredentials);

            return new TokenData()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = token.ValidTo
            };
        }
    }
}