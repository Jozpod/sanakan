using Sanakan.Api.Models;
using System.Security.Claims;

namespace Sanakan
{
    public interface IJwtBuilder
    {
        TokenData Build(params Claim[] claims);
    }
}
