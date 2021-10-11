using System;
using System.Net.Http;

namespace Shinden.API
{
    public abstract class QueryGet<T> : Query<T>, IGet where T : class
    {
        // IRequest
        public override HttpRequestMessage Message => new HttpRequestMessage 
        {
            RequestUri = new Uri(Uri),
            Method = HttpMethod.Get
        };
    }
}
