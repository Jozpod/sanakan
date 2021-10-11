using System.Net.Http;
using Newtonsoft.Json;

namespace Shinden.API
{
    public abstract class Query<T> : IQuery<T> where T : class
    {
        protected string Token;

        // IQuery
        public abstract string Uri { get; }
        public abstract string QueryUri { get; }
        public abstract HttpRequestMessage Message { get; }
        public string BaseUri => "https://api.shinden.pl/api/";

        public void WithToken(string token) => Token = token;
        public virtual T Parse(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}
