using System.Collections.Generic;
using System.Threading.Tasks;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public class SearchModule
    {
        private readonly RequestManager _manager;

        public SearchModule(RequestManager manager)
        {
            _manager = manager;
        }

        public async Task<Response<List<IUserSearch>>> UserAsync(string nick)
        {
            var raw = await _manager.QueryAsync(new SearchUserQuery(nick)).ConfigureAwait(false);
            return new ResponseFinal<List<IUserSearch>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IPersonSearch>>> CharacterAsync(string name)
        {
            var raw = await _manager.QueryAsync(new SearchCharacterQuery(name)).ConfigureAwait(false);
            return new ResponseFinal<List<IPersonSearch>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IPersonSearch>>> StaffAsync(string name)
        {
            var raw = await _manager.QueryAsync(new SearchStaffQuery(name)).ConfigureAwait(false);
            return new ResponseFinal<List<IPersonSearch>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IQuickSearch>>> QuickSearchAsync(string search)
        {
            var anime = await QuickSearchAnimeAsync(search).ConfigureAwait(false);
            var manga = await QuickSearchMangaAsync(search).ConfigureAwait(false);

            var response = new ResponseFinal<List<IQuickSearch>>(500, null);
            var list = new List<IQuickSearch>();

            if (anime.IsSuccessStatusCode())
            {
                list.AddRange(anime.Body);
                response.SetCode(anime.Code);
                response.SetBody(list);
            }
            if (manga.IsSuccessStatusCode())
            {
                list.AddRange(manga.Body);
                response.SetCode(manga.Code);
                response.SetBody(list);
            }

            return response;
        }

        public async Task<Response<List<IQuickSearch>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            switch (type)
            {
                case QuickSearchType.Anime: return await QuickSearchAnimeAsync(search).ConfigureAwait(false);
                case QuickSearchType.Manga: return await QuickSearchMangaAsync(search).ConfigureAwait(false);
                default: return new ResponseFinal<List<IQuickSearch>>(500, null);
            }
        }

        private async Task<Response<List<IQuickSearch>>> QuickSearchAnimeAsync(string title)
        {
            var raw = await _manager.QueryAsync(new QuickSearchAnimeQuery(title)).ConfigureAwait(false);
            return new ResponseFinal<List<IQuickSearch>>(raw.Code, raw.Body?.ToModel(QuickSearchType.Anime));
        }

        private async Task<Response<List<IQuickSearch>>> QuickSearchMangaAsync(string title)
        {
            var raw = await _manager.QueryAsync(new QuickSearchMangaQuery(title)).ConfigureAwait(false);
            return new ResponseFinal<List<IQuickSearch>>(raw.Code, raw.Body?.ToModel(QuickSearchType.Manga));
        }
    }
}