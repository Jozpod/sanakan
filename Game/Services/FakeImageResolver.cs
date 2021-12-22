using Sanakan.Common;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.Game.Services
{
    public class FakeImageResolver : IImageResolver
    {
        private readonly IFileSystem _fileSystem;

        public FakeImageResolver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public async Task<Stream?> GetAsync(string url)
        {
            return new MemoryStream();
        }
    }
}
