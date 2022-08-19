using Sanakan.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.Game.Services
{
    [ExcludeFromCodeCoverage]
    public class FakeImageResolver : IImageResolver
    {
        private readonly HttpClient _httpClient;
        private readonly IFileSystem _fileSystem;

        public FakeImageResolver(
            IHttpClientFactory httpClientFactory,
            IFileSystem fileSystem)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(ImageResolver));
            _fileSystem = fileSystem;
        }

        public async Task<Stream?> GetAsync(Uri url)
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                return stream;
            }

            return new MemoryStream();
        }
    }
}
