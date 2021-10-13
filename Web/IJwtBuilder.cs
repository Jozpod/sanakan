using Sanakan.Api.Models;

namespace Sanakan
{
    public interface IJwtBuilder
    {
        TokenData Build(ulong userId);
    }
}
