using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IUserRepository :
        ICreateRepository<User>, IRemoveRepository<User>, ISaveRepository
    {
        Task<List<ulong>> GetShindenUserIdsByHavingCharacterAsync(ulong id);

        Task<User?> GetWithWaifuProfileAsync(ulong shindenUserId);

        Task<User?> GetBasicAsync(ulong discordUserId);

        Task<User?> GetUserAndDontTrackAsync(ulong discordUserId);

        Task<User?> GetUserOrCreateAsync(ulong discordUserId);

        Task<User?> GetByShindenIdAsync(ulong shindenUserId);

        Task<User?> GetByShindenIdAsync(ulong shindenUserId, UserQueryOptions userQueryOptions);

        Task<User?> GetCachedAsync(ulong shindenUserId);

        Task<User?> GetCachedByShindenIdAsync(ulong shindenUserId);

        Task<List<User>> GetCachedAllUsersLiteAsync();

        Task<IEnumerable<User>> GetAllCachedAsync();

        Task<bool> ExistsByDiscordIdAsync(ulong discordUserId);

        Task<User?> GetByDiscordIdAsync(ulong discordUserId);

        Task<bool> ExistsByShindenIdAsync(ulong shindenUserId);

        Task<List<User>> GetByShindenIdExcludeDiscordIdAsync(ulong shindenUserId, ulong discordUserId);

        Task<List<ulong>> GetByExcludedDiscordIdsAsync(IEnumerable<ulong> discordUserIds);
    }
}
