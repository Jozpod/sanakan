using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.IntegrationTests
{
    public class FakeShindenClient : IShindenClient
    {
        public Task<Result<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleStatusAfterChange>> ChangeTitleStatusAsync(ulong userId, ListType status, ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ulong>>> GetAllCharactersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<CharacterInfo>> GetCharacterInfoAsync(ulong characterId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleCharacters>> GetCharactersAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleEpisodes>> GetEpisodesAsync(ulong episodeId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<NewEpisode>>> GetNewEpisodesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleRecommendation>> GetRecommendationsAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleRelations>> GetRelationsAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleReviews>> GetReviewsAsync(ulong reviewId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<StaffInfo>> GetStaffInfoAsync(ulong staffId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserInfo>> GetUserInfoAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<LogInResult>> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<CharacterSearchResult>>> SearchCharacterAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Result<List<UserSearchResult>>> SearchUserAsync(string nick)
        {
            throw new NotImplementedException();
        }
    }
}
