using System.Net;

namespace Shinden.API
{
    public interface IResponse<T>
    {
         T Body { get; }
         HttpStatusCode Code { get; }

         bool IsSuccessStatusCode();
    }
}