using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IUserRepository : 
        ICreateRepository<User>, IRemoveRepository<User>, ISaveRepository
    {
        Task<IEnumerable<ulong>> GetUserShindenIdsByHavingCharacterAsync(ulong id);
        Task<User?> GetUserWaifuProfileAsync(ulong shindenUserId);
        Task<User?> GetBaseUserAndDontTrackAsync(ulong discordUserId);
        Task<User?> GetUserAndDontTrackAsync(ulong discordUserId);
        Task<User?> GetUserOrCreateAsync(ulong discordUserId);
        Task<User?> GetByShindenIdAsync(ulong userShindenId);
        Task<User?> GetByShindenIdAsync(ulong userShindenId, UserQueryOptions userQueryOptions);
        Task<User?> GetCachedFullUserAsync(ulong userId);
        Task<User?> GetCachedFullUserByShindenIdAsync(ulong userId);
        Task<List<User>> GetCachedAllUsersLiteAsync();
        Task<List<User>> GetCachedAllUsersAsync();
        Task<bool> ExistsByDiscordIdAsync(ulong userId);
        Task<User?> GetByDiscordIdAsync(ulong discordUserId);
        Task<bool> ExistsByShindenIdAsync(ulong id);
        Task<List<User>> GetByShindenIdExcludeDiscordIdAsync(ulong shindenUserId, ulong discordUserId);
        Task<List<ulong>> GetByExcludedDiscordIdsAsync(IEnumerable<ulong> discordUserIds);
    }
}
