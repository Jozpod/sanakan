using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Web.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Sanakan.Api
{
    internal class JwtBuilder : IJwtBuilder
    {
        private readonly Encoding _encoding;
        private readonly IOptionsMonitor<JwtConfiguration> _options;
        private readonly ISystemClock _systemClock;
        private readonly SigningCredentials _signingCredentials;
        private readonly SecurityTokenHandler _securityTokenHandler;

        public JwtBuilder(
            IOptionsMonitor<JwtConfiguration> options,
            Encoding encoding,
            ISystemClock systemClock)
        {
            _options = options;
            _encoding = encoding;
            _systemClock = systemClock;
            var securityKey = new SymmetricSecurityKey(_encoding.GetBytes(_options.CurrentValue.IssuerSigningKey));
            _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            _securityTokenHandler = new JwtSecurityTokenHandler();
        }

        public static SecurityKey ToSecurityKey(string key) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        public TokenData Build(TimeSpan expiresOn, params Claim[] claims)
        {
            var options = _options.CurrentValue;
            var allClaims = claims.Append(
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            var token = new JwtSecurityToken(
                options.Issuer,
                options.Issuer,
                allClaims,
                expires: _systemClock.UtcNow + expiresOn,
                signingCredentials: _signingCredentials);

            return new TokenData()
            {
                Token = _securityTokenHandler.WriteToken(token),
                Expire = token.ValidTo
            };
        }
    }
}