using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shinden;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;
using Shinden.Modules;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public class ShindenClient : IShindenClient
    {
        private readonly ILogger<ShindenClient> _logger;

        //public TitleModule Title { get; }
        //public SearchModule Search { get; }
        //public ExperimentalModule Ex { get; }
        //public LoggedInUserModule.UserModule User { get; }

        public ShindenClient(
            IOptions<ShindenClientOptions> options,
            ILogger<ShindenClient> logger)
        {
            //Auth authenticator,
            //_logger.LogInformation($"Runing as: {authenticator.GetUserAgent()}");
            //_manager = new RequestManager(authenticator, logger);

            //Title = new TitleModule(_manager);
            //Search = new SearchModule(_manager);
            //Ex = new ExperimentalModule(_manager);
            //User = new LoggedInUserModule.UserModule(_manager, _logger);
        }

        public async Task<Response<List<INewEpisode>>> GetNewEpisodesAsync()
        {
            var raw = await QueryAsync(new GetNewEpisodesQuery()).ConfigureAwait(false);
            return new ResponseFinal<List<INewEpisode>>(raw.Code, new List<INewEpisode>(raw.Body?.ToModel()));
        }

        public async Task<Response<IStaffInfo>> GetStaffInfoAsync(ulong id)
        {
            var raw = await QueryAsync(new GetStaffInfoQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<IStaffInfo>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IStaffInfo>> GetStaffInfoAsync(IIndexable id)
        {
            return await GetStaffInfoAsync(id.Id).ConfigureAwait(false);
        }

        public async Task<Response<ICharacterInfo>> GetCharacterInfoAsync(ulong id)
        {
            var raw = await QueryAsync(new GetCharacterfInfoQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<ICharacterInfo>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<ICharacterInfo>> GetCharacterInfoAsync(IIndexable id)
        {
            return await GetCharacterInfoAsync(id.Id).ConfigureAwait(false);
        }

        public void WithUserSession(ISession session)
        {
            _session = session;
            AddSessionCookies();
            _logger.Log(LogLevel.Information, "User session has been crated.");
        }

        public async Task<IResponse<T>> QueryAsync<T>(IQuery<T> query) where T : class
        {
            await CheckQuery(query);

            using var handler = new HttpClientHandler() { CookieContainer = _cookies };
            using var client = new HttpClient(handler);
            query.WithToken(_auth.Token);

            client.DefaultRequestHeaders.Add("Accept-Language", "pl");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", $"{_auth.GetUserAgent()}");

            if (_auth.Marmolade != null)
            {
                client.DefaultRequestHeaders.Add(_auth.Marmolade, "marmolada");
            }

            _logger.Log(LogLevel.Information, $"Processing request: [{query.Message.Method}] {query.Uri}");

            if (query.Message.Content != null)
            {
                _logger.Log(LogLevel.Trace, $"Request body: {await query.Message.Content.ReadAsStringAsync()}");
            }

            var response = await client.SendAsync(query.Message).ConfigureAwait(false);

            if (response == null)
            {
                _logger.Log(LogLevel.Critical, "Null response!");
                throw new Exception("Null response!");
            }

            _logger.Log(LogLevel.Information, $"Response code: {(int)response.StatusCode}");

            var final = new ResponseFinal<T>(response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.Log(response.IsSuccessStatusCode ? LogLevel.Trace : LogLevel.Debug, $"Response body: {responseBody}");

            if (!response.IsSuccessStatusCode) return final;
            _logger.Log(LogLevel.Debug, "Parsing response.");

            try
            {
                final.SetBody(query.Parse(responseBody));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"In parsing: {ex}");
            }

            return final;
        }

        private async Task CheckQuery<T>(IQuery<T> query) where T : class
        {
            if (query is LoginUserQuery)
            {
                if (_baseUri == null)
                    _baseUri = new Uri(query.BaseUri);
            }
            else
                await CheckSession();
        }

        private async Task CheckSession()
        {
            if (_session != null)
            {
                if (!_session.IsValid())
                {
                    var nS = await QueryAsync(new LoginUserQuery(_session.GetAuth())).ConfigureAwait(false);
                    if (nS.IsSuccessStatusCode())
                    {
                        _logger.Log(LogLevel.Information, "User session has been renewed.");
                        _session.Renew(nS.Body.ToModel(_session.GetAuth()).Session);
                        AddSessionCookies();
                    }
                }
            }
        }

        private void AddSessionCookies()
        {
            if (_session == null || _baseUri == null || !_session.IsValid())
            {
                return;
            }

            _cookies.Add(_baseUri, new Cookie() { Name = "name", Value = _session.Name, Expires = _session.Expires });
            _cookies.Add(_baseUri, new Cookie() { Name = "id", Value = _session.Id, Expires = _session.Expires });
        }

        #region Title
        public async Task<Response<IEpisodesRange>> GetEpisodesRangeAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleEpisodesRangeQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<IEpisodesRange>(raw.Code, raw.Body?.ToModel(id));
        }

        public async Task<Response<IEpisodesRange>> GetEpisodesRangeAsync(IIndexable index)
        {
            return await GetEpisodesRangeAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IEpisode>>> GetEpisodesAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleEpisodesQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IEpisode>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IEpisode>>> GetEpisodesAsync(IIndexable index)
        {
            return await GetEpisodesAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<ITitleInfo>> GetInfoAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleInfoQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<ITitleInfo>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<ITitleInfo>> GetInfoAsync(IIndexable index)
        {
            return await GetInfoAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IRecommendation>>> GetRecommendationsAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleRecommendationsQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IRecommendation>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IRecommendation>>> GetRecommendationsAsync(IIndexable index)
        {
            return await GetRecommendationsAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IReview>>> GetReviewsAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleReviewsQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IReview>>(raw.Code, raw.Body?.ToModel(id));
        }

        public async Task<Response<List<IReview>>> GetReviewsAsync(IIndexable index)
        {
            return await GetReviewsAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<ITitleRelation>>> GetRelationsAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleRelatedQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<ITitleRelation>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<ITitleRelation>>> GetRelationsAsync(IIndexable index)
        {
            return await GetRelationsAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<List<IRelation>>> GetCharactersAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleCharactersQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IRelation>>(raw.Code, raw.Body?.Relations?.ToModel());
        }

        public async Task<Response<List<IRelation>>> GetCharactersAsync(IIndexable index)
        {
            return await GetCharactersAsync(index.Id).ConfigureAwait(false);
        }
        #endregion
        #region Search
        public async Task<Response<List<IUserSearch>>> UserAsync(string nick)
        {
            var raw = await QueryAsync(new SearchUserQuery(nick)).ConfigureAwait(false);
            return new ResponseFinal<List<IUserSearch>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IPersonSearch>>> CharacterAsync(string name)
        {
            var raw = await QueryAsync(new SearchCharacterQuery(name)).ConfigureAwait(false);
            return new ResponseFinal<List<IPersonSearch>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IPersonSearch>>> StaffAsync(string name)
        {
            var raw = await QueryAsync(new SearchStaffQuery(name)).ConfigureAwait(false);
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
            var raw = await QueryAsync(new QuickSearchAnimeQuery(title)).ConfigureAwait(false);
            return new ResponseFinal<List<IQuickSearch>>(raw.Code, raw.Body?.ToModel(QuickSearchType.Anime));
        }

        private async Task<Response<List<IQuickSearch>>> QuickSearchMangaAsync(string title)
        {
            var raw = await QueryAsync(new QuickSearchMangaQuery(title)).ConfigureAwait(false);
            return new ResponseFinal<List<IQuickSearch>>(raw.Code, raw.Body?.ToModel(QuickSearchType.Manga));
        }
        #endregion
        #region Experimental
        public async Task<Response<List<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            var raw = await QueryAsync(new GetAllAnimeCharacters()).ConfigureAwait(false);
            return new ResponseFinal<List<ulong>>(raw.Code, raw.Body);
        }

        public async Task<Response<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            var raw = await QueryAsync(new GetAllMangaCharacters()).ConfigureAwait(false);
            return new ResponseFinal<List<ulong>>(raw.Code, raw.Body);
        }

        public async Task<Response<List<ulong>>> GetAllCharactersAsync()
        {
            var raw = await QueryAsync(new GetAllCharacters()).ConfigureAwait(false);
            return new ResponseFinal<List<ulong>>(raw.Code, raw.Body);
        }
        #endregion
        #region User
        public async Task<Response<IUserInfo>> LoginAsync(UserAuth auth)
        {
            return await LoginAsync(auth).ConfigureAwait(false);
        }

        public async Task<Response<IUserInfo>> GetAsync(ulong id)
        {
            var raw = await QueryAsync(new GetUserInfoQuery(id)).ConfigureAwait(false);
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
            var raw = await QueryAsync(new GetUserFavCharactersQuery(userId)).ConfigureAwait(false);
            return new ResponseFinal<List<ICharacterInfoShort>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<ILastWatched>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            var raw = await QueryAsync(new GetLastWatchedQuery(userId, limit)).ConfigureAwait(false);
            return new ResponseFinal<List<ILastWatched>>(raw.Code, raw.Body?.ToAnimeModel());
        }

        public async Task<Response<List<ILastReaded>>> GetLastReadedAsync(IIndexable index, uint limit = 5)
        {
            return await GetLastReadedAsync(index.Id, limit).ConfigureAwait(false);
        }

        public async Task<Response<List<ILastReaded>>> GetLastReadedAsync(ulong userId, uint limit = 5)
        {
            var raw = await QueryAsync(new GetLastReadedQuery(userId, limit)).ConfigureAwait(false);
            return new ResponseFinal<List<ILastReaded>>(raw.Code, raw.Body?.ToMangaModel());
        }
        #endregion
        #region LoggedIn
        protected async Task<Response<IUserInfo>> LoginAsync(UserAuth auth)
        {
            var raw = await QueryAsync(new LoginUserQuery(auth)).ConfigureAwait(false);

            if (raw.IsSuccessStatusCode())
            {
                _loggedUser = raw.Body?.ToModel(auth);
                WithUserSession(_loggedUser?.Session);
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
            var raw = await QueryAsync(new ChangeTitleStatusQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> ChangeTitleStatusAsync(ListType status, IIndexable index)
        {
            return await ChangeTitleStatusAsync(status, index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RemoveTitleFromListAsync(ulong id)
        {
            var raw = await QueryAsync(new RemoveTitleFromListQuery(Get().Id, id)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RemoveTitleFromListAsync(IIndexable index)
        {
            return await RemoveTitleFromListAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> IncreaseNumberOfWatchedEpisodesAsync(ulong id)
        {
            var raw = await QueryAsync(new IncreaseWatchedEpisode(Get().Id, id)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> IncreaseNumberOfWatchedEpisodesAsync(IIndexable index)
        {
            return await IncreaseNumberOfWatchedEpisodesAsync(index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            var config = new RateAnimeConfig() { TitleId = titleId, RateType = type, RateValue = value };
            var raw = await QueryAsync(new RateAnimeQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RateAnimeAsync(IIndexable index, AnimeRateType type, uint value)
        {
            return await RateAnimeAsync(index.Id, type, value).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            var config = new RateMangaConfig() { TitleId = titleId, RateType = type, RateValue = value };
            var raw = await QueryAsync(new RateMangaQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RateMangaAsync(IIndexable index, MangaRateType type, uint value)
        {
            return await RateMangaAsync(index.Id, type, value).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> AddToFavouritesAsync(FavouriteType type, ulong id)
        {
            var config = new FavouriteConfig() { FavouriteId = id, Type = type, UserId = Get().Id };
            var raw = await QueryAsync(new AddToFavouriteQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> AddToFavouritesAsync(FavouriteType type, IIndexable index)
        {
            return await AddToFavouritesAsync(type, index.Id).ConfigureAwait(false);
        }

        public async Task<Response<IEmptyResponse>> RemoveFromFavouritesAsync(FavouriteType type, ulong id)
        {
            var config = new FavouriteConfig() { FavouriteId = id, Type = type, UserId = Get().Id };
            var raw = await QueryAsync(new RemoveFromFavouriteQuery(config)).ConfigureAwait(false);
            return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<IEmptyResponse>> RemoveFromFavouritesAsync(FavouriteType type, IIndexable index)
        {
            return await RemoveFromFavouritesAsync(type, index.Id).ConfigureAwait(false);
        }
        #endregion
    }
}