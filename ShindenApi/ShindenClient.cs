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

        private class Credentials
        {
            public string Username { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;
        }

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

        private async Task TryRenewSessionAsync()
        {
            if (_expiresOn < _systemClock.UtcNow)
            {
                if (_credentials == null)
                {
                    return;
                }

                var _ = await LoginAsync(_credentials.Username, _credentials.Password);
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

        public async Task<Result<List<NewEpisode>>> GetNewEpisodesAsync()
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
                return new Result<List<NewEpisode>>();
            }

            var result = await response.Content.ReadFromJsonAsync<NewEpisodeRoot>(_jsonSerializerOptions);

            return new Result<List<NewEpisode>>()
            {
                Value = result.LastOnline,
            };
        }

        public async Task<Result<StaffInfo>> GetStaffInfoAsync(ulong staffId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"staff/{staffId}", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<StaffInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<StaffInfo>(_jsonSerializerOptions);

            return new Result<StaffInfo>()
            {
                Value = result
            };
        }

        public async Task<Result<CharacterInfo>> GetCharacterInfoAsync(ulong characterId)
        {
            await TryRenewSessionAsync();

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

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<CharacterInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<CharacterInfo>(_jsonSerializerOptions);

            return new Result<CharacterInfo>()
            {
                Value = result
            };
        }

        #region Title
        public async Task<Result<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<EpisodesRange>(_jsonSerializerOptions);

            return new Result<EpisodesRange>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleEpisodes>> GetEpisodesAsync(ulong episodeId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{episodeId}/episodes", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleEpisodes>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleEpisodes>(_jsonSerializerOptions);

            return new Result<TitleEpisodes>()
            {
                Value = result
            };
        }

        public async Task<Result<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "lang", "pl" },
                { "decode", 1.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/info", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<AnimeMangaInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<AnimeMangaInfo>(_jsonSerializerOptions);

            return new Result<AnimeMangaInfo>()
            {
                Value = result
            };
        }

        public async Task<Result<IllustrationInfo>> GetIllustrationInfoAsync(ulong titleId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "lang", "pl" },
                { "decode", 1.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/info", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<IllustrationInfo>();
            }

            var result = await response.Content.ReadFromJsonAsync<IllustrationInfo>(_jsonSerializerOptions);

            return new Result<IllustrationInfo>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleRecommendation>> GetRecommendationsAsync(ulong titleId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/recommendations", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleRecommendation>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleRecommendation>(_jsonSerializerOptions);

            return new Result<TitleRecommendation>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleReviews>> GetReviewsAsync(ulong reviewId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{reviewId}/reviews", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleReviews>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleReviews>(_jsonSerializerOptions);

            return new Result<TitleReviews>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleRelations>> GetRelationsAsync(ulong titleId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/related", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleRelations>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleRelations>(_jsonSerializerOptions);

            return new Result<TitleRelations>()
            {
                Value = result
            };
        }

        public async Task<Result<TitleCharacters>> GetCharactersAsync(ulong titleId)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
            };

            var query = QueryHelpers.AddQueryString($"title/{titleId}/characters", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<TitleCharacters>();
            }

            var result = await response.Content.ReadFromJsonAsync<TitleCharacters>(_jsonSerializerOptions);

            return new Result<TitleCharacters>()
            {
                Value = result
            };
        }

        #endregion
        #region Search
        public async Task<Result<List<UserSearchResult>>> SearchUserAsync(string nick)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<UserSearchResult>>(_jsonSerializerOptions);

            return new Result<List<UserSearchResult>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<CharacterSearchResult>>> SearchCharacterAsync(string name)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<CharacterSearchResult>>(_jsonSerializerOptions);

            return new Result<List<CharacterSearchResult>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<StaffSearchResult>>> SearchStaffAsync(string name)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<StaffSearchResult>>(_jsonSerializerOptions);

            return new Result<List<StaffSearchResult>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search)
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
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<QuickSearchResult>>(_jsonSerializerOptions);

            return new Result<List<QuickSearchResult>>()
            {
                Value = result
            };
        }

        private async Task<Result<List<QuickSearchResult>>> QuickSearchMangaAsync(string title)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<QuickSearchResult>>(_jsonSerializerOptions);

            return new Result<List<QuickSearchResult>>()
            {
                Value = result
            };
        }
        #endregion
        #region Experimental
        public async Task<Result<List<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<ulong>>(_jsonSerializerOptions);

            return new Result<List<ulong>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<ulong>>(_jsonSerializerOptions);

            return new Result<List<ulong>>()
            {
                Value = result
            };
        }

        public async Task<Result<List<ulong>>> GetAllCharactersAsync()
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<ulong>>(_jsonSerializerOptions);

            return new Result<List<ulong>>()
            {
                Value = result
            };
        }
        #endregion
        #region User

        public async Task<Result<UserInfo>> GetUserInfoAsync(ulong userId)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<UserInfo>(_jsonSerializerOptions);

            return new Result<UserInfo>
            {
                Value = result
            };
        }

        public async Task<Result<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<List<FavCharacter>>(_jsonSerializerOptions);

            return new Result<List<FavCharacter>>
            {
                Value = result,
            };
        }

        public async Task<Result<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "limit", limit.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/last_view", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<LastWatchedRead>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<LastWatchedRead>>(_jsonSerializerOptions);

            return new Result<List<LastWatchedRead>>
            {
                Value = result,
            };
        }

        public async Task<Result<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5)
        {
            await TryRenewSessionAsync();

            var queryData = new Dictionary<string, string>()
            {
                { "api_key", _options.CurrentValue.Token },
                { "limit", limit.ToString() },
            };

            var query = QueryHelpers.AddQueryString($"user/{userId}/last_read", queryData);

            var response = await _httpClient.GetAsync(query);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<List<LastWatchedRead>>();
            }

            var result = await response.Content.ReadFromJsonAsync<List<LastWatchedRead>>(_jsonSerializerOptions);

            return new Result<List<LastWatchedRead>>
            {
                Value = result,
            };
        }
        #endregion
        #region LoggedIn
        public async Task<Result<LogInResult>> LoginAsync(string username, string password)
        {
            var formData = new Dictionary<string, string?>()
            {
                { nameof(username), username },
                { nameof(password), password },
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

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

            var result = await response.Content.ReadFromJsonAsync<LogInResult>(_jsonSerializerOptions);
            _credentials = new Credentials
            {
                Username = username,
                Password = username
            };
            var session = result.Session;
            SetSession(session);

            return new Result<LogInResult>
            {
                Value = result,
            };
        }
        public async Task<Result<TitleStatusAfterChange>> ChangeTitleStatusAsync(
            ulong userId,
            ListType status,
            ulong titleId)
        {
            await TryRenewSessionAsync();

            var formData = new Dictionary<string, string?>()
            {
                { nameof(status), status.ToQuery() },
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

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

            var result = await response.Content.ReadFromJsonAsync<TitleStatusAfterChange>(_jsonSerializerOptions);

            return new Result<TitleStatusAfterChange>
            {
                Value = result,
            };
        }

        public async Task<Result<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<TitleStatusAfterChange>(_jsonSerializerOptions);

            return new Result<TitleStatusAfterChange>
            {
                Value = result,
            };
        }

        public async Task<Result<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            await TryRenewSessionAsync();

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

            var result = await response.Content.ReadFromJsonAsync<IncreaseWatched>(_jsonSerializerOptions);

            return new Result<IncreaseWatched>
            {
                Value = result,
            };
        }

        public async Task<Result<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            await TryRenewSessionAsync();

            var formData = new Dictionary<string, string>()
            {
                { nameof(type), type.ToQuery() },
                { nameof(value), value.ToString() },
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

            var response = await _httpClient.PostAsync($"anime/{titleId}/rate", content);

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Status>();
            }

            var result = await response.Content.ReadFromJsonAsync<Status>(_jsonSerializerOptions);

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
            await TryRenewSessionAsync();

            var formData = new Dictionary<string, string>()
            {
                { "type", ToQuery(type) },
                { "value", value.ToString() }
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

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

            var result = await response.Content.ReadFromJsonAsync<Status>(_jsonSerializerOptions);

            return new Result<Status>()
            {
                Value = result
            };
        }

        public async Task<Result<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType type, ulong id)
        {
            await TryRenewSessionAsync();

            var formData = new Dictionary<string, string?>()
            {
                { "id", $"{type.ToString().ToLower()}-{id}" }
            };

            var content = new FormUrlEncodedContent(formData!);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            content.Headers.ContentType.CharSet = "UTF-8";

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

            var result = await response.Content.ReadFromJsonAsync<Modification>(_jsonSerializerOptions);

            return new Result<Modification>()
            {
                Value = result
            };
        }

        public async Task<Result<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType type, ulong favouriteId)
        {
            await TryRenewSessionAsync();

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

            var response = await _httpClient.SendAsync(new HttpRequestMessage
            {
                RequestUri = new Uri(query, UriKind.Relative),
                Method = HttpMethod.Delete,
                Content = content,
            });

            if (!response.IsSuccessStatusCode)
            {
                return new Result<Modification>();
            }

            var result = await response.Content.ReadFromJsonAsync<Modification>(_jsonSerializerOptions);

            return new Result<Modification>()
            {
                Value = result
            };
        }
        #endregion
    }
}