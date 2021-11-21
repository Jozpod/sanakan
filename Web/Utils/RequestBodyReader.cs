using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web
{
    internal class RequestBodyReader : IRequestBodyReader
    {
        private readonly Encoding _encoding;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestBodyReader(
            Encoding encoding,
            IHttpContextAccessor httpContextAccessor)
        {
            _encoding = encoding;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetStringAsync()
        {
            using var reader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body, _encoding);
            return await reader.ReadToEndAsync();
        }
    }
}
