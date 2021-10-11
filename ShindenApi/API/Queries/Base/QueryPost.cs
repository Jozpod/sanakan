using System;
using System.Net.Http;

namespace Shinden.API
{
    public abstract class QueryPost<T> : Query<T>, IPost where T : class
    {
        // IPost
        public virtual HttpContent Content => null;

        // IRequest
        public override HttpRequestMessage Message => new HttpRequestMessage
        {
            RequestUri = new Uri(Uri),
            Method = HttpMethod.Post,
            Content = Content
        };
    }
}
