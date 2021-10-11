using System.Collections.Generic;
using System.Threading.Tasks;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public class ExperimentalModule
    {
        private readonly RequestManager _manager;

        public ExperimentalModule(RequestManager manager)
        {
            _manager = manager;
        }

        public async Task<Response<List<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            var raw = await _manager.QueryAsync(new GetAllAnimeCharacters()).ConfigureAwait(false);
            return new ResponseFinal<List<ulong>>(raw.Code, raw.Body);
        }

        public async Task<Response<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            var raw = await _manager.QueryAsync(new GetAllMangaCharacters()).ConfigureAwait(false);
            return new ResponseFinal<List<ulong>>(raw.Code, raw.Body);
        }

        public async Task<Response<List<ulong>>> GetAllCharactersAsync()
        {
            var raw = await _manager.QueryAsync(new GetAllCharacters()).ConfigureAwait(false);
            return new ResponseFinal<List<ulong>>(raw.Code, raw.Body);
        }
    }
}