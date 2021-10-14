using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public partial class LoggedInUserModule
    {
        public class UserModule
        {
            private readonly RequestManager _manager;
            private readonly ILogger _logger;

            public LoggedInUserModule OnLoggedIn { get; set; }

            public UserModule(RequestManager manager, ILogger logger)
            {
                _logger = logger;
                _manager = manager;

                OnLoggedIn = new LoggedInUserModule(_manager, _logger);
            }

            public async Task<Response<IUserInfo>> LoginAsync(UserAuth auth)
            {
                return await OnLoggedIn.LoginAsync(auth).ConfigureAwait(false);
            }

            public async Task<Response<IUserInfo>> GetAsync(ulong id)
            {
                var raw = await _manager.QueryAsync(new GetUserInfoQuery(id)).ConfigureAwait(false);
                return new ResponseFinal<IUserInfo>(raw.Code, raw.Body?.ToModel());
            }

            public async Task<Response<IUserInfo>> GetAsync(IIndexable index)
            {
                return await GetAsync(index.Id).ConfigureAwait(false);
            }

            public async Task<Response<List<ILastWatched>>> GetLastWatchedAsync(IIndexable index, uint limit = 5)
            {
                return await GetLastWatchedAsync(index.Id, limit).ConfigureAwait(false);
            }

            public async Task<Response<List<ICharacterInfoShort>>> GetFavCharactersAsync(IIndexable index)
            {
                return await GetFavCharactersAsync(index.Id).ConfigureAwait(false);
            }

            public async Task<Response<List<ICharacterInfoShort>>> GetFavCharactersAsync(ulong userId)
            {
                var raw = await _manager.QueryAsync(new GetUserFavCharactersQuery(userId)).ConfigureAwait(false);
                return new ResponseFinal<List<ICharacterInfoShort>>(raw.Code, raw.Body?.ToModel());
            }

            public async Task<Response<List<ILastWatched>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
            {
                var raw = await _manager.QueryAsync(new GetLastWatchedQuery(userId, limit)).ConfigureAwait(false);
                return new ResponseFinal<List<ILastWatched>>(raw.Code, raw.Body?.ToAnimeModel());
            }

            public async Task<Response<List<ILastReaded>>> GetLastReadedAsync(IIndexable index, uint limit = 5)
            {
                return await GetLastReadedAsync(index.Id, limit).ConfigureAwait(false);
            }

            public async Task<Response<List<ILastReaded>>> GetLastReadedAsync(ulong userId, uint limit = 5)
            {
                var raw = await _manager.QueryAsync(new GetLastReadedQuery(userId, limit)).ConfigureAwait(false);
                return new ResponseFinal<List<ILastReaded>>(raw.Code, raw.Body?.ToMangaModel());
            }
        }
    }
}