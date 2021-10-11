#pragma warning disable 1591

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Api.Models;
using Sanakan.Config;

namespace Sanakan.Api
{
    public class UserTokenBuilder
    {
        private readonly Encoding _encoding;
        private readonly ISystemClock systemClock;

        public UserTokenBuilder(
            IOptions<> options,
            Encoding encoding,
            ISystemClock systemClock)
        {
            _encoding = encoding; // Encoding.UTF8
            _securityKey = new SymmetricSecurityKey(_encoding.GetBytes(config.Jwt.Key));
            _systemClock = systemClock;
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public static TokenData BuildUserToken(User user)
        {
            var config = conf.Get();

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("DiscordId", user.Id.ToString()),
                new Claim("Player", "waifu_player"),
            };

            var token = new JwtSecurityToken(config.Jwt.Issuer,
              config.Jwt.Issuer,
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: _signingCredentials);

            return new TokenData()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expire = token.ValidTo
            };
        }
    }
}