using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.Game.Services
{
    [ExcludeFromCodeCoverage]
    public class ImageResolver : IImageResolver
    {
        private readonly HttpClient _httpClient;

        public ImageResolver(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(ImageResolver));
        }

        public async Task<Stream?> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }

            return null;
        }
    }
}
