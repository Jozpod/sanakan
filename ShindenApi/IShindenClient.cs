﻿using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using Shinden.API;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public interface IShindenClient
    {
        Task<Result<StaffInfo>> GetStaffInfoAsync(ulong staffId);
        Task<Result<TitleEpisodes>> GetEpisodesAsync(ulong episodeId);
        Task<Result<LogInResult>> LoginAsync(string username, string password);
        Task<Result<List<ulong>>> GetAllCharactersFromMangaAsync();
        Task<Result<List<ulong>>> GetAllCharactersAsync();
        Task<Result<List<UserSearchResult>>> SearchUserAsync(string nick);
        Task<Result<TitleRecommendation>> GetRecommendationsAsync(ulong titleId);
        Task<Result<List<CharacterSearchResult>>> SearchCharacterAsync(string name);
        Task<Result<List<NewEpisode>>> GetNewEpisodesAsync();
        Task<Result<TitleCharacters>> GetCharactersAsync(ulong titleId);
        Task<Result<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong titleId);
        Task<Result<TitleReviews>> GetReviewsAsync(ulong reviewId);
        Task<Result<TitleRelations>> GetRelationsAsync(ulong titleId);
        Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type);
        Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search);
        Task<Result<UserInfo>> GetUserInfoAsync(ulong userId);
        Task<Result<List<LastWatchedRead>>> GetLastWatchedAsync(ulong userId, uint limit = 5);
        Task<Result<List<LastWatchedRead>>> GetLastReadAsync(ulong userId, uint limit = 5);
        Task<Result<List<ulong>>> GetAllCharactersFromAnimeAsync();
        Task<Result<List<FavCharacter>>> GetFavouriteCharactersAsync(ulong userId);
        Task<Result<CharacterInfo>> GetCharacterInfoAsync(ulong characterId);
    }
}