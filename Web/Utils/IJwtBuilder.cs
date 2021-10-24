using Sanakan.Api.Models;
using System;
using System.Security.Claims;

namespace Sanakan
{
    public interface IJwtBuilder
    {
        TokenData Build(TimeSpan expiresOn, params Claim[] claims);
    }
}
