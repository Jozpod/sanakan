using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public interface IShindenClient
    {
        Task<ShindenResult<Modification>> RemoveFromFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId);

        Task<ShindenResult<Modification>> AddToFavouritesAsync(ulong userId, FavouriteType favouriteType, ulong favouriteId);

        Task<ShindenResult<Status>> RateMangaAsync(ulong titleId, MangaRateType type, uint value);

        Task<ShindenResult<Status>> RateAnimeAsync(ulong titleId, AnimeRateType type, uint value);

        Task<ShindenResult<IncreaseWatched>> IncreaseNumberOfWatchedEpisodesAsync(ulong userId, ulong titleId);

        Task<ShindenResult<TitleStatusAfterChange>> RemoveTitleFromListAsync(ulong userId, ulong titleId);

        Task<ShindenResult<TitleStatusAfterChange>> ChangeTitleStatusAsync(ulong userId, ListType status, ulong titleId);

        Task<ShindenResult<EpisodesRange>> GetEpisodesRangeAsync(ulong episodeId);

        Task<ShindenResult<StaffInfo>> GetStaffInfoAsync(ulong staffId);

        Task<ShindenResult<TitleEpisodes>> GetEpisodesAsync(ulong episodeId);

        Task<ShindenResult<LogInResult>> LoginAsync(string username, string password);

        Task<ShindenResult<List<ulong>>> GetAllCharactersFromMangaAsync();

        Task<ShindenResult<List<ulong>>> GetAllCharactersAsync();

        Task<ShindenResult<List<UserSearchResult>>> SearchUserAsync(string nick);

        Task<ShindenResult<TitleRecommendation>> GetRecommendationsAsync(ulong titleId);

        Task<ShindenResult<List<CharacterSearchResult>>> SearchCharacterAsync(string name);

        Task<ShindenResult<List<NewEpisode>>> GetNewEpisodesAsync();

        Task<ShindenResult<TitleCharacters>> GetCharactersAsync(ulong titleId);

        Task<ShindenResult<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId);

        Task<ShindenResult<IllustrationInfo>> GetIllustrationInfoAsync(ulong titleId);

        Task<ShindenResult<TitleReviews>> GetReviewsAsync(ulong reviewId);

        Task<ShindenResult<TitleRelations>> GetRelationsAsync(ulong titleId);

        Task<ShindenResult<List<StaffSearchResult>>> SearchStaffAsync(string name);

        Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type);

        Task<ShindenResult<List<QuickSearchResult>>> QuickSearchAsync(string search);

        Task<ShindenResult<UserInfo>> GetUserInfoAsync(ulong userId);

        Task<ShindenResult<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5);

        Task<ShindenResult<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5);

        Task<ShindenResult<IEnumerable<ulong>>> GetAllCharactersFromAnimeAsync();

        Task<ShindenResult<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId);

        Task<ShindenResult<CharacterInfo>> GetCharacterInfoAsync(ulong characterId);
    }
}
