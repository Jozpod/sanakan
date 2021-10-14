using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Api.Models;

namespace Sanakan.Api
{
    public class JwtBuilder : IJwtBuilder
    {
        private readonly Encoding _encoding;
        private readonly JwtConfig _config;
        private readonly ISystemClock _systemClock;

        public JwtBuilder(
            IOptions<JwtConfig> options,
            Encoding encoding,
            ISystemClock systemClock)
        {
            _encoding = encoding; // Encoding.UTF8
            _securityKey = new SymmetricSecurityKey(_encoding.GetBytes(config.Jwt.Key));
            _systemClock = systemClock;
            _options = options;
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public TokenData Build(param Claim[] claims)
        {
            var config = conf.Get();

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("DiscordId", userId.ToString()),
                new Claim("Player", "waifu_player"),
            };

            var token = new JwtSecurityToken(config.Issuer,
              config.Issuer,
              claims,
              expires: _systemClock.Now + _config, //DateTime.Now.AddMinutes(30),
              signingCredentials: _signingCredentials);

            return new TokenData()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = token.ValidTo
            };
        }
    }
}