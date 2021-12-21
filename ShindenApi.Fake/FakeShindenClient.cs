using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake
{
    internal class FakeShindenClient : IShindenClient
    {
        private readonly WebScrapedDbContext _dbContext;

        public FakeShindenClient(WebScrapedDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ShindenResult<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            return Task.FromResult(new ShindenResult<Modification>
            {
                Value = new Modification
                {
                    Updated = string.Empty,
                }
            });
        }

        public Task<ShindenResult<TitleStatusAfterChange>> ChangeTitleStatusAsync(ulong userId, ListType status, ulong titleId)
        {
            return Task.FromResult(new ShindenResult<TitleStatusAfterChange>
            {
                Value = new TitleStatusAfterChange
                {
                    
                }
            });
        }

        public Task<ShindenResult<List<ulong>>> GetAllCharactersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<IEnumerable<ulong>>> GetAllCharactersFromAnimeAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<ulong>>> GetAllCharactersFromMangaAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<CharacterInfo>> GetCharacterInfoAsync(ulong characterId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleCharacters>> GetCharactersAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleEpisodes>> GetEpisodesAsync(ulong episodeId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<IllustrationInfo>> GetIllustrationInfoAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<NewEpisode>>> GetNewEpisodesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleRecommendation>> GetRecommendationsAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleRelations>> GetRelationsAsync(ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<TitleReviews>> GetReviewsAsync(ulong reviewId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<StaffInfo>> GetStaffInfoAsync(ulong staffId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<UserInfo>> GetUserInfoAsync(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<LogInResult>> LoginAsync(string username, string password)
        {
            return Task.FromResult(new ShindenResult<LogInResult>
            {
                Value = new LogInResult
                {

                }
            });
        }

        public Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value)
        {
            return Task.FromResult(new ShindenResult<Status>
            {
                Value = new Status
                {

                }
            });
        }

        public Task<ShindenResult<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value)
        {
            return Task.FromResult(new ShindenResult<Status>
            {
                Value = new Status
                {

                }
            });
        }

        public Task<ShindenResult<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId)
        {
            return Task.FromResult(new ShindenResult<Modification>
            {
                Value = new Modification
                {

                }
            });
        }

        public Task<ShindenResult<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId)
        {
            return Task.FromResult(new ShindenResult<TitleStatusAfterChange>
            {
                Value = new TitleStatusAfterChange
                {

                }
            });
        }

        public Task<ShindenResult<List<CharacterSearchResult>>> SearchCharacterAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<StaffSearchResult>>> SearchStaffAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<ShindenResult<List<UserSearchResult>>> SearchUserAsync(string nick)
        {
            throw new NotImplementedException();
        }
    }
}
