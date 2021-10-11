using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Logger;
using Shinden.Logger.In;
using Shinden.Models;

namespace Shinden.Modules
{
    public partial class LoggedInUserModule
    {
        private readonly RequestManager _manager;
        private readonly IInLogger _logger;
        private ILoggedUser _loggedUser;

        public LoggedInUserModule(RequestManager manager, IInLogger logger)
        {
            _logger = logger;
            _manager = manager;
            _loggedUser = null;
        }

        protected async Task<Response<IUserInfo>> LoginAsync(UserAuth auth)
        {
            var raw = await _manager.QueryAsync(new LoginUserQuery(auth)).ConfigureAwait(false);
            if (raw.IsSuccessStatusCode())
            {
                _loggedUser = raw.Body?.ToModel(auth);
                _manager.WithUserSession(_loggedUser?.Session);
            }
            return new ResponseFinal<IUserInfo>(raw.Code, Get());
        }

        public IUserInfo Get()
        {
            if (_loggedUser == null)
            {
                _logger.Log(LogLevel.Critical, "User is not logged in!");
                throw new Exception("User is not logged in!");
            }
            return _loggedUser;
        }

        public async Task<Response<IEmptyResponse>> ChangeTitleStatusAsync(ListType status, ulong id)
        {
            var config = new ChangeTitleStatusConfig() { TitleId = id, NewListType = status, UserId = Get().Id };
            var raw = await _manager.QueryAsync(new ChangeTitleStatusQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> ChangeTitleStatusAsync(ListType status, IIndexable index)
        {
            return await ChangeTitleStatusAsync(status, index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RemoveTitleFromListAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new RemoveTitleFromListQuery(Get().Id, id)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RemoveTitleFromListAsync(IIndexable index)
        {
            return await RemoveTitleFromListAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> IncreaseNumberOfWatchedEpisodesAsync(ulong id)
        {
            var raw = await _manager.QueryAsync(new IncreaseWatchedEpisode(Get().Id, id)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> IncreaseNumberOfWatchedEpisodesAsync(IIndexable index)
        {
            return await IncreaseNumberOfWatchedEpisodesAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            var config = new RateAnimeConfig() { TitleId = titleId, RateType = type, RateValue = value };
            var raw = await _manager.QueryAsync(new RateAnimeQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RateAnimeAsync(IIndexable index, AnimeRateType type, uint value)
        {
            return await RateAnimeAsync(index.Id, type, value).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            var config = new RateMangaConfig() { TitleId = titleId, RateType = type, RateValue = value };
            var raw = await _manager.QueryAsync(new RateMangaQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RateMangaAsync(IIndexable index, MangaRateType type, uint value)
        {
            return await RateMangaAsync(index.Id, type, value).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> AddToFavouritesAsync(FavouriteType type, ulong id)
        {
            var config = new FavouriteConfig() { FavouriteId = id, Type = type, UserId = Get().Id };
            var raw = await _manager.QueryAsync(new AddToFavouriteQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> AddToFavouritesAsync(FavouriteType type, IIndexable index)
        {
            return await AddToFavouritesAsync(type, index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RemoveFromFavouritesAsync(FavouriteType type, ulong id)
        {
            var config = new FavouriteConfig() { FavouriteId = id, Type = type, UserId = Get().Id };
            var raw = await _manager.QueryAsync(new RemoveFromFavouriteQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RemoveFromFavouritesAsync(FavouriteType type, IIndexable index)
        {
            return await RemoveFromFavouritesAsync(type, index.Id).ConfigureAwait(false);
        }
    }
}