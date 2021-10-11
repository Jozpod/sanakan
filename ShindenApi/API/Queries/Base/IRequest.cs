using System.Net.Http;

namespace Shinden.API
{
    public interface IRequest
    {
        HttpRequestMessage Message { get; }
    }
}
