using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sanakan.Common.Configuration;
using Shinden;
using Shinden.API;
using Shinden.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public class ShindenClient : IShindenClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private readonly IOptionsMonitor<ShindenApiConfiguration> _options;
        private readonly ILogger _logger;

        public ShindenClient(
            HttpClient httpClient,
            CookieContainer cookieContainer,
            IOptionsMonitor<ShindenApiConfiguration> options,
            ILogger<ShindenClient> logger)
        {
            //$"{auth.UserAgent} (Shinden.NET/{Assembly.GetAssembly(typeof(ShindenClient)).GetName().Version})"

            //_cookies = new CookieContainer();
            //_cookies.Add(_baseUri, new Cookie() { 
            //    Name = "name",
            //    Value = _session.Name,
            //    Expires = _session.Expires
            //});
            //_cookies.Add(_baseUri, new Cookie() {
            //    Name = "id",
            //    Value = _session.Id,
            //    Expires = _session.Expires
            //});
            //var handler = new HttpClientHandler() { CookieContainer = _cookies };
            //new HttpClient(handler);
            _httpClient = httpClient;
            _cookieContainer = cookieContainer;
            _options = options;
            _logger = logger;
        }

        public async Task<Result<List<NewEpisode>>> GetNewEpisodesAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString("episode/new", queryData);

            var response = await _httpClient.GetAsync(query);

            if(!response.IsSuccessStatusCode)
            {
                return new Result<List<NewEpisode>>();
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

            var result = await response.Content.ReadFromJsonAsync<StaffInfo>();

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
        public async Task<Result<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"episode/{episodeId}/range", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<EpisodesRange>();
            }

            var result = await response.Content.ReadFromJsonAsync<EpisodesRange>();

            return new Result<EpisodesRange>()
            {
                Value = result
            };
        }

        public async Task<Result<List<TitleEpisodes>>> GetEpisodesAsync(ulong episodeId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{episodeId}/episodes", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<TitleEpisodes>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<TitleEpisodes>>();

            return new Result<List<TitleEpisodes>>()
            {
                Value = result
            };
        }

        public async Task<Result<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong id)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "lang", "pl" },
                { "decode", 1.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"title/{id}/info", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<AnimeMangaInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<AnimeMangaInfo>();

            return new Result<AnimeMangaInfo>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleRecommendation>> GetRecommendationsAsync(ulong id)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{id}/recommendations", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleRecommendation>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleRecommendation>();

            return new Result<TitleRecommendation>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleReviews>> GetReviewsAsync(ulong id)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{id}/reviews", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleReviews>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleReviews>();

            return new Result<TitleReviews>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleRelations>> GetRelationsAsync(ulong id)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{id}/related", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleRelations>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleRelations>();

            return new Result<TitleRelations>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleCharacters>> GetCharactersAsync(ulong id)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{id}/characters", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleCharacters>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleCharacters>();

            return new Result<TitleCharacters>()
            {
                Value = result
            };
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

        public async Task<Result<UserInfo>> GetUserInfoAsync(ulong userId)
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

        public async Task<Result<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId)
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
        public async Task<Result<LogInResult>> LoginAsync(string username, string password)
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
                return new Result<LogInResult>();
            }

            var result = await response.Content.ReadFromJsonAsync<LogInResult>();

            return new Result<LogInResult>
            {
                Value = result,
            };
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