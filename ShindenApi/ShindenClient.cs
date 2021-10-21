using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shinden;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;
using Shinden.Modules;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public class ShindenClient : IShindenClient
    {
        private readonly Uri _baseUri;
        private readonly IOptionsMonitor<ShindenClientOptions> _options;
        private readonly ILogger _logger;
        private readonly CookieContainer _cookies;
        private readonly HttpClient _httpClient;
        private ILoggedUser _loggedUser;
        private ISession _session;

        public ShindenClient(
            IOptionsMonitor<ShindenClientOptions> options,
            ILogger<ShindenClient> logger)
        {
            _cookies = new CookieContainer();
            _cookies.Add(_baseUri, new Cookie() { Name = "name", Value = _session.Name, Expires = _session.Expires });
            _cookies.Add(_baseUri, new Cookie() { Name = "id", Value = _session.Id, Expires = _session.Expires });
            var handler = new HttpClientHandler() { CookieContainer = _cookies };
            _httpClient = new HttpClient(handler);


            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "pl");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"{options.CurrentValue.UserAgent}");

            if (options.CurrentValue.Marmolade != null)
            {
                _httpClient.DefaultRequestHeaders.Add(options.CurrentValue.Marmolade, "marmolada");
            }
        }

        public async Task<object> GetNewEpisodesAsync()
        {
            var response = await _httpClient.GetAsync("{BaseUri}episode/new?api_key={Token}");

            if(!response.IsSuccessStatusCode)
            {
                return new Result<object>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var list = new List<NewEpisode>();
            var jsonObj = JObject.Parse(json);
            foreach (var item in jsonObj["lastonline"].Children())
            {
                list.Add(JsonConvert.DeserializeObject<NewEpisode>(item.ToString()));
            }

            return new Result<List<NewEpisode>>()
            {
                Value = list
            };

            //var raw = await QueryAsync(new GetNewEpisodesQuery()).ConfigureAwait(false);
            //return new ResponseFinal<List<INewEpisode>>(raw.Code, new List<INewEpisode>(raw.Body?.ToModel()));
        }

        public async Task<object> GetStaffInfoAsync(ulong id)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"staff/{id}", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<object>();
            }

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<StaffInfo>(json);

            return new Result<StaffInfo>()
            {
                Value = result
            };
        }

        public async Task<Result<CharacterInfo>> GetCharacterInfoAsync(ulong id)
        {
            var oneToStr = 1.ToString();
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "fav_stats", oneToStr },
                { "relations", oneToStr },
                { "points", oneToStr },
                { "bio", oneToStr },
                { "pictures", oneToStr },
            };

            var query = QueryHelpers.AddQueryString($"character/{id}", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<CharacterInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<CharacterInfo>();

            return new Result<CharacterInfo>()
            {
                Value = result
            };
        }

        #region Title
        public async Task<Response<IEpisodesRange>> GetEpisodesRangeAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleEpisodesRangeQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<IEpisodesRange>(raw.Code, raw.Body?.ToModel(id));
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

        public async Task<Response<List<IRecommendation>>> GetRecommendationsAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleRecommendationsQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IRecommendation>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Response<List<IReview>>> GetReviewsAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleReviewsQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<IReview>>(raw.Code, raw.Body?.ToModel(id));
        }

        public async Task<Response<List<ITitleRelation>>> GetRelationsAsync(ulong id)
        {
            var raw = await QueryAsync(new GetTitleRelatedQuery(id)).ConfigureAwait(false);
            return new ResponseFinal<List<ITitleRelation>>(raw.Code, raw.Body?.ToModel());
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
        public async Task<Result<List<UserSearchResult>>> SearchUserAsync(string nick)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "query", nick },
            };

            var query = QueryHelpers.AddQueryString($"user/search", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<UserSearchResult>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<UserSearchResult>>();

            return new Result<List<UserSearchResult>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<CharacterSearchResult>>> SearchCharacterAsync(string name)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "query", name },
            };

            var query = QueryHelpers.AddQueryString($"character/search", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<CharacterSearchResult>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<CharacterSearchResult>>();

            return new Result<List<CharacterSearchResult>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<StaffSearchResult>>> SearchStaffAsync(string name)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "query", name },
            };

            var query = QueryHelpers.AddQueryString($"staff/search", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<StaffSearchResult>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<StaffSearchResult>>();

            return new Result<List<StaffSearchResult>>()
            {
                Value = result
            };

            //var raw = await QueryAsync(new SearchStaffQuery(name)).ConfigureAwait(false);
            //return new ResponseFinal<List<IPersonSearch>>(raw.Code, raw.Body?.ToModel());
        }

        public async Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search)
        {
            var anime = await QuickSearchAnimeAsync(search).ConfigureAwait(false);
            var manga = await QuickSearchMangaAsync(search).ConfigureAwait(false);
            var list = new List<QuickSearchResult>();

            if (anime.Value != null)
            {
                list.AddRange(anime.Value);
            }

            if (manga.Value != null)
            {
                list.AddRange(manga.Value);
            }

            return new Result<List<QuickSearchResult>>
            {
                Value = list,
            };
        }

        public async Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            switch (type)
            {
                case QuickSearchType.Anime: return await QuickSearchAnimeAsync(search).ConfigureAwait(false);
                case QuickSearchType.Manga: return await QuickSearchMangaAsync(search).ConfigureAwait(false);
                default: return new Result<List<QuickSearchResult>>();
            }
        }

        private async Task<Result<List<QuickSearchResult>>> QuickSearchAnimeAsync(string title)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "accepted_types", "Anime" },
                { "decode", 1.ToString() },
                { "query", title },
            };

            var query = QueryHelpers.AddQueryString($"title/search", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<QuickSearchResult>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<QuickSearchResult>>();

            return new Result<List<QuickSearchResult>>()
            {
                Value = result
            };
        }

        private async Task<Result<List<QuickSearchResult>>> QuickSearchMangaAsync(string title)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "accepted_types", "Manga;Manhua;Novel;Doujin;Manhwa;OEL;One+Shot" },
                { "decode", 1.ToString() },
                { "query", title },
            };

            var query = QueryHelpers.AddQueryString($"title/search", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<QuickSearchResult>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<QuickSearchResult>>();

            return new Result<List<QuickSearchResult>>()
            {
                Value = result
            };
        }
        #endregion
        #region Experimental
        public async Task<Result<List<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"character/in-anime", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<ulong>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<ulong>>();

            return new Result<List<ulong>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"character/in-manga", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<ulong>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<ulong>>();

            return new Result<List<ulong>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<ulong>>> GetAllCharactersAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"character/in-manga", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<ulong>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<ulong>>();

            return new Result<List<ulong>>()
            {
                Value = result
            };
        }
        #endregion
        #region User

        public async Task<Result<UserInfo>> GetAsync(ulong userId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/info", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<UserInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<UserInfo>();

            return new Result<UserInfo>
            {
                Value = result
            };
        }

        public async Task<Result<List<FavCharacter>>> GetFavCharactersAsync(ulong userId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/fav-chars", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<FavCharacter>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<FavCharacter>>();

            return new Result<List<FavCharacter>>
            {
                Value = result,
            };
        }

        public async Task<Result<List<LastWatchedReaded>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "limit", limit.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/last_view", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<LastWatchedReaded>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<LastWatchedReaded>>();

            return new Result<List<LastWatchedReaded>>
            {
                Value = result,
            };
        }

        public async Task<Result<List<LastWatchedReaded>>> GetLastReadedAsync(ulong userId, uint limit = 5)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "limit", limit.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/last_read", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<LastWatchedReaded>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<LastWatchedReaded>>();

            return new Result<List<LastWatchedReaded>>
            {
                Value = result,
            };
        }
        #endregion
        #region LoggedIn
        public async Task<Result<Logging>> LoginAsync(string username, string password)
        {
            var formData = new Dictionary<string, string>()
            {
                { nameof(username), username },
                { nameof(password), password },
            };

            var content = new FormUrlEncodedContent(formData);

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString("user/login", queryData);

            var response = await _httpClient.PostAsync(query, content);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Logging>();
            }

            var result = await response.Content.ReadFromJsonAsync<Logging>();

            return new Result<Logging>
            {
                Value = result,
            };
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

        public async Task<Result<TitleStatusAfterChange>> ChangeTitleStatusAsync(
            ulong userId, ListType status, ulong titleId)
        {
            var formData = new Dictionary<string, string>()
            {
                { nameof(status), status.ToQuery() },
            };

            var content = new FormUrlEncodedContent(formData);

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/series/{titleId}", queryData);

            var response = await _httpClient.PostAsync(query, content);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleStatusAfterChange>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleStatusAfterChange>();

            return new Result<TitleStatusAfterChange>
            {
                Value = result,
            };

            //var config = new ChangeTitleStatusConfig() { TitleId = id, NewListType = status, UserId = Get().Id };
            //var raw = await QueryAsync(new ChangeTitleStatusQuery(config)).ConfigureAwait(false);
            //return new ResponseFinal<IEmptyResponse>(raw.Code, raw.Body?.ToModel());
        }

        //public async Task<Response<IEmptyResponse>> ChangeTitleStatusAsync(ListType status, IIndexable index)
        //{
        //    return await ChangeTitleStatusAsync(status, index.Id).ConfigureAwait(false);
        //}

        public async Task<Result<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/series/{titleId}", queryData);

            var response = await _httpClient.DeleteAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleStatusAfterChange>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleStatusAfterChange>();

            return new Result<TitleStatusAfterChange>
            {
                Value = result,
            };
        }

        public async Task<Result<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            userId = Get().Id;
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/increase-watched/{titleId}", queryData);

            var response = await _httpClient.PostAsync(query, null);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<IncreaseWatched>();
            }

            var result = await response.Content.ReadFromJsonAsync<IncreaseWatched>();

            return new Result<IncreaseWatched>
            {
                Value = result,
            };
        }

        public async Task<Result<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            var formData = new Dictionary<string, string>()
            {
                { nameof(type), type.ToQuery() },
                { nameof(value), value.ToString() },
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.PostAsync($"anime/{titleId}/rate", content);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Status>();
            }

            var result = await response.Content.ReadFromJsonAsync<Status>();

            return new Result<Status>()
            {
                Value = result
            };
        }

        public string ToQuery(MangaRateType type)
        {
            switch (type)
            {
                case MangaRateType.Characters: return "titlecahracters";
                case MangaRateType.Story: return "story";
                case MangaRateType.Total: return "total";
                case MangaRateType.Art: return "lines";
                default: return "total";
            }
        }

        public async Task<Result<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            var formData = new Dictionary<string, string>()
            {
                { "type", ToQuery(type) },
                { "value", value.ToString() }
            };

            var content = new FormUrlEncodedContent(formData);

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"manga/{titleId}/rate", queryData);

            var response = await _httpClient.PostAsync(query, content);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Status>();
            }

            var result = await response.Content.ReadFromJsonAsync<Status>();

            return new Result<Status>()
            {
                Value = result
            };
        }

        public async Task<Result<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType type, ulong id)
        {
            var formData = new Dictionary<string, string>()
            {
                { "id", $"{type.ToString().ToLower()}-{id}" }
            };

            var content = new FormUrlEncodedContent(formData);

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/fav", queryData);

            var response = await _httpClient.PostAsync(query, content);            

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Modification>();
            }

            var result = await response.Content.ReadFromJsonAsync<Modification>();

            return new Result<Modification>()
            {
                Value = result
            };
        }

        public async Task<Result<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType type, ulong favouriteId)
        {
            var formData = new Dictionary<string, string>()
            {
                { "id", $"type.ToQuery(favouriteId)" }
            };

            var content = new FormUrlEncodedContent(formData);

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/fav ", queryData);

            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(query),
                Method = HttpMethod.Delete,
                Content = content,
            });

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Modification>();
            }

            var result = await response.Content.ReadFromJsonAsync<Modification>();

            return new Result<Modification>()
            {
                Value = result
            };
        }
        #endregion
    }
}