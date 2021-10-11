using System.Net.Http;

namespace Shinden.API
{
    public interface IPost : IRequest
    {
        string Uri { get; }
        HttpContent Content { get; }
    }
}
