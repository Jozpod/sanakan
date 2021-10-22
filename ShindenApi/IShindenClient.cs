using Shinden;
using Shinden.API;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi
{
    public interface IShindenClient
    {
        Task<Result<List<UserSearchResult>>> SearchUserAsync(string nick);
        Task<Result<List<CharacterSearchResult>>> SearchCharacterAsync(string name);
        Task<Result<List<NewEpisode>>> GetNewEpisodesAsync();
        Task<Result<TitleCharacters>> GetCharactersAsync(ulong id);
        Task<Result<AnimeMangaInfo>> GetAnimeMangaInfoAsync(ulong id);
        Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search, QuickSearchType type);
        Task<Result<List<QuickSearchResult>>> QuickSearchAsync(string search);
        Task<Result<UserInfo>> GetUserInfoAsync(ulong userId);
        Task<Result<List<LastWatchedReaded>>> GetLastWatchedAsync(ulong userId, uint limit = 5);
        Task<Result<List<LastWatchedReaded>>> GetLastReadedAsync(ulong userId, uint limit = 5);
        Task<Result<List<ulong>>> GetAllCharactersFromAnimeAsync();
        Task<Result<List<FavCharacter>>> GetFavCharactersAsync(ulong userId);
        Task<Result<CharacterInfo>> GetCharacterInfoAsync(ulong id);
    }
}
