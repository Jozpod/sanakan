using System.Net.Http;

namespace Shinden.API
{
    public interface IDelete : IRequest
    {
        string Uri { get; }
        HttpContent Content { get; }
    }
}
