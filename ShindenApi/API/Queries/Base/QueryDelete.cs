using System;
using System.Net.Http;

namespace Shinden.API
{
    public abstract class QueryDelete<T> : Query<T>, IDelete where T : class
    {
        // IDelete
        public virtual HttpContent Content => null;

        // IRequest
        public override HttpRequestMessage Message => new HttpRequestMessage
        {
            RequestUri = new Uri(Uri),
            Method = HttpMethod.Delete,
            Content = Content
        };
    }
}
