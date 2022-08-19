using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    internal class ShindenClient : IShindenClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private readonly IOptionsMonitor<ShindenApiConfiguration> _options;
        private readonly ISystemClock _systemClock;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private Credentials? _credentials;
        private DateTime _expiresOn;

        public ShindenClient(
            HttpClient httpClient,
            CookieContainer cookieContainer,
            IOptionsMonitor<ShindenApiConfiguration> options,
            ISystemClock systemClock,
            ILogger<ShindenClient> logger)
        {
            _httpClient = httpClient;
            _cookieContainer = cookieContainer;
            _options = options;
            _systemClock = systemClock;
            _logger = logger;
            _expiresOn = DateTime.MinValue;
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
            };
            _jsonSerializerOptions.Converters.Add(new MangaStatusConverter());
            _jsonSerializerOptions.Converters.Add(new AnimeStatusConverter());
            _jsonSerializerOptions.Converters.Add(new GenderConverter());
            _jsonSerializerOptions.Converters.Add(new LanguageConverter());
            _jsonSerializerOptions.Converters.Add(new MpaaRatingConverter());
            _jsonSerializerOptions.Converters.Add(new PictureTypeConverter());
            _jsonSerializerOptions.Converters.Add(new AnimeTypeConverter());
            _jsonSerializerOptions.Converters.Add(new MangaTypeConverter());
            _jsonSerializerOptions.Converters.Add(new AlternativeTitleTypeConverter());
            _jsonSerializerOptions.Converters.Add(new UserStatusConverter());
            _jsonSerializerOptions.Converters.Add(new StaffTypeConverter());
            _jsonSerializerOptions.Converters.Add(new EpisodeTypeConverter());
            _jsonSerializerOptions.Converters.Add(new IllustrationTypeConverter());
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            _jsonSerializerOptions.Converters.Add(new IllustrationConverter());
        }

        public async Task<ShindenResult<T>> SendAsync<T>(string requestUri, HttpMethod? httpMethod = null, HttpContent? httpContent = null)
        {
            await TryRenewSessionAsync();
            httpMethod ??= HttpMethod.Get;

            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Content = httpContent ?? request.Content;

            var response = await _httpClient.SendAsync(request);
            var shindenResult = new ShindenResult<T>
            {
                StatusCode = response.StatusCode
            };

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Could not access {0}", requestUri);
                shindenResult.RawText = await response.Content.ReadAsStringAsync();
                return shindenResult;
            }

            try
            {
                var result = await response.Content.ReadFromJsonAsync<T>(_jsonSerializerOptions);
                shindenResult.Value = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while parsing json");
                shindenResult.ParseError = ex.Message;
            }

            return shindenResult;
        }

        public async Task<ShindenResult<List<NewEpisode>>> GetNewEpisodesAsync()
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString("episode/new", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new ShindenResult<List<NewEpisode>>();
            }

            var result = await response.Content.ReadFromJsonAsync<NewEpisodeRoot>(_jsonSerializerOptions);

            return new ShindenResult<List<NewEpisode>>()
            {
                Value = result.LastOnline,
            };
        }

        public Task<ShindenResult<StaffInfo>> GetStaffInfoAsync(ulong staffId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"staff/{staffId}", queryData);
            return SendAsync<StaffInfo>(query);
        }

        public Task<ShindenResult<CharacterInfo>> GetCharacterInfoAsync(ulong characterId)
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

            var query = QueryHelpers.AddQueryString($"character/{characterId}", queryData);

            return SendAsync<CharacterInfo>(query);
        }

        #region Title
        public Task<ShindenResult<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"episode/{episodeId}/range", queryData);

            return SendAsync<EpisodesRange>(query);
        }

        public Task<ShindenResult<TitleEpisodes>> GetEpisodesAsync(ulong episodeId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{episodeId}/episodes", queryData);

            return SendAsync<TitleEpisodes>(query);
        }

        public Task<ShindenResult<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "lang", "pl" },
                { "decode", 1.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/info", queryData);

            return SendAsync<AnimeMangaInfo>(query);
        }

        public Task<ShindenResult<IllustrationInfo>> GetIllustrationInfoAsync(ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "lang", "pl" },
                { "decode", 1.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/info", queryData);

            return SendAsync<IllustrationInfo>(query);
        }

        public Task<ShindenResult<TitleRecommendation>> GetRecommendationsAsync(ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/recommendations", queryData);

            return SendAsync<TitleRecommendation>(query);
        }

        public Task<ShindenResult<TitleReviews>> GetReviewsAsync(ulong reviewId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{reviewId}/reviews", queryData);

            return SendAsync<TitleReviews>(query);
        }

        public Task<ShindenResult<TitleRelations>> GetRelationsAsync(ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/related", queryData);

            return SendAsync<TitleRelations>(query);
        }

        public Task<ShindenResult<TitleCharacters>> GetCharactersAsync(ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/characters", queryData);

            return SendAsync<TitleCharacters>(query);
        }

        #endregion
        #region Search
        public Task<ShindenResult<List<UserSearchResult>>> SearchUserAsync(string nick)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "query", nick },
            };

            var query = QueryHelpers.AddQueryString($"user/search", queryData);

            return SendAsync<List<UserSearchResult>>(query);
        }

        public Task<ShindenResult<List<CharacterSearchResult>>> SearchCharacterAsync(string name)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "query", name },
            };

            var query = QueryHelpers.AddQueryString($"character/search", queryData);
            return SendAsync<List<CharacterSearchResult>>(query);
        }

        public Task<ShindenResult<List<StaffSearchResult>>> SearchStaffAsync(string name)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "query", name },
            };

            var query = QueryHelpers.AddQueryString($"staff/search", queryData);

            return SendAsync<List<StaffSearchResult>>(query);
        }

        public async Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search)
        {
            await TryRenewSessionAsync();
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

            return new ShindenResult<List<QuickSearchResult>>
            {
                Value = list,
            };
        }

        public async Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            switch (type)
            {
                case QuickSearchType.Anime: return await QuickSearchAnimeAsync(search).ConfigureAwait(false);
                case QuickSearchType.Manga: return await QuickSearchMangaAsync(search).ConfigureAwait(false);
                default: return new ShindenResult<List<QuickSearchResult>>();
            }
        }

        #endregion
        #region Experimental
        public Task<ShindenResult<IEnumerable<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"character/in-anime", queryData);

            return SendAsync<IEnumerable<ulong>>(query);
        }

        public Task<ShindenResult<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"character/in-manga", queryData);

            return SendAsync<List<ulong>>(query);
        }

        public Task<ShindenResult<List<ulong>>> GetAllCharactersAsync()
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"character/in-manga", queryData);

            return SendAsync<List<ulong>>(query);
        }
        #endregion
        #region User

        public Task<ShindenResult<UserInfo>> GetUserInfoAsync(ulong userId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/info", queryData);

            return SendAsync<UserInfo>(query);
        }

        public Task<ShindenResult<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/fav-chars", queryData);

            return SendAsync<List<FavCharacter>>(query);
        }

        public Task<ShindenResult<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "limit", limit.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/last_view", queryData);

            return SendAsync<List<LastWatchedRead>>(query);
        }

        public Task<ShindenResult<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "limit", limit.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/last_read", queryData);

            return SendAsync<List<LastWatchedRead>>(query);
        }

        #endregion
        #region LoggedIn
        public async Task<ShindenResult<LogInResult>> LoginAsync(string username, string password)
        {
            var formData = new Dictionary<string, string?>()
            {
                { nameof(username), username },
                { nameof(password), password },
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "UTF-8"
            };

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString("user/login", queryData);

            var logInResult = await SendAsync<LogInResult>(query, HttpMethod.Post);

            if (logInResult.Value == null)
            {
                return logInResult;
            }

            _credentials = new Credentials
            {
                Username = username,
                Password = username
            };
            var session = logInResult.Value.Session;
            SetSession(session);

            return logInResult;
        }

        public Task<ShindenResult<TitleStatusAfterChange>> ChangeTitleStatusAsync(
            ulong userId,
            ListType status,
            ulong titleId)
        {
            var formData = new Dictionary<string, string?>()
            {
                { nameof(status), status.ToQuery() },
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "UTF-8"
            };

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/series/{titleId}", queryData);

            return SendAsync<TitleStatusAfterChange>(query, HttpMethod.Post, content);
        }

        public Task<ShindenResult<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/series/{titleId}", queryData);

            return SendAsync<TitleStatusAfterChange>(query, HttpMethod.Delete);
        }

        public Task<ShindenResult<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/increase-watched/{titleId}", queryData);

            return SendAsync<IncreaseWatched>(query, HttpMethod.Post);
        }

        public Task<ShindenResult<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            var formData = new Dictionary<string, string>()
            {
                { nameof(type), type.ToQuery() },
                { nameof(value), value.ToString() },
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "UTF-8"
            };
            var query = $"anime/{titleId}/rate";

            return SendAsync<Status>(query, HttpMethod.Post, content);
        }

        public Task<ShindenResult<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            var formData = new Dictionary<string, string>()
            {
                { "type", type.ToQuery() },
                { "value", value.ToString() }
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "UTF-8"
            };

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"manga/{titleId}/rate", queryData);

            return SendAsync<Status>(query, HttpMethod.Post, content);
        }

        public Task<ShindenResult<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType type, ulong id)
        {
            var formData = new Dictionary<string, string?>()
            {
                { "id", $"{type.ToString().ToLower()}-{id}" }
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
            {
                CharSet = "UTF-8"
            };

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token }
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/fav", queryData);
            return SendAsync<Modification>(query, HttpMethod.Post, content);
        }

        public Task<ShindenResult<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType type, ulong favouriteId)
        {
            var formData = new Dictionary<string, string>()
            {
                { "id", $"{type.ToString().ToLower()}-{favouriteId}" }
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"userlist/{userId}/fav", queryData);

            return SendAsync<Modification>(query, HttpMethod.Delete, content);
        }
        #endregion

        private Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAnimeAsync(string title)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "accepted_types", "Anime" },
                { "decode", 1.ToString() },
                { "query", title },
            };

            var query = QueryHelpers.AddQueryString($"title/search", queryData);
            return SendAsync<List<QuickSearchResult>>(query);
        }

        private Task<ShindenResult<List<QuickSearchResult>>> QuickSearchMangaAsync(string title)
        {
            var queryData = new Dictionary<string, string>()
            {
                { "accepted_types", "Manga;Manhua;Novel;Doujin;Manhwa;OEL;One+Shot" },
                { "decode", 1.ToString() },
                { "query", title },
            };

            var query = QueryHelpers.AddQueryString($"title/search", queryData);

            return SendAsync<List<QuickSearchResult>>(query);
        }

        private async Task TryRenewSessionAsync()
        {
            if (_expiresOn < _systemClock.UtcNow)
            {
                if (_credentials == null)
                {
                    return;
                }

                await LoginAsync(_credentials.Username, _credentials.Password);
            }
        }

        private void SetSession(LogInResultSession session)
        {
            _expiresOn = _systemClock.UtcNow + _options.CurrentValue.SessionExiry;
            var baseAddress = _httpClient.BaseAddress!;

            _cookieContainer.Add(baseAddress, new Cookie()
            {
                Name = "name",
                Value = session.Name,
                Expires = _expiresOn,
            });
            _cookieContainer.Add(baseAddress, new Cookie()
            {
                Name = "id",
                Value = session.Id,
                Expires = _expiresOn,
            });
        }

        private class Credentials
        {
            public string Username { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;
        }
    }
}