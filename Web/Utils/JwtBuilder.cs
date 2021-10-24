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
        private readonly IOptionsMonitor<JwtConfig> _options;
        private readonly ISystemClock _systemClock;
        private readonly SigningCredentials _signingCredentials;

        public JwtBuilder(
            IOptionsMonitor<JwtConfig> options,
            Encoding encoding,
            ISystemClock systemClock)
        {
            _options = options;
            _encoding = encoding;
            _systemClock = systemClock;
            var securityKey = new SymmetricSecurityKey(_encoding.GetBytes(_options.CurrentValue.Key));
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        public TokenData Build(TimeSpan expiresOn, params Claim[] claims)
        {
            var options = _options.CurrentValue;
            var allClaims = claims.Append(
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var token = new JwtSecurityToken(options.Issuer,
                options.Issuer,
                allClaims,
                expires: _systemClock.UtcNow + expiresOn,
                signingCredentials: _signingCredentials);

            return new TokenData()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = token.ValidTo
            };
        }
    }
}