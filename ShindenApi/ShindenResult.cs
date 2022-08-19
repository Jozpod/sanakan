using System.Net;

namespace Sanakan.ShindenApi
{
    public class ShindenResult<T>
    {
        public T? Value { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string? RawText { get; set; }

        public string? ParseError { get; set; }
    }
}
