using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IUserRepository
    {
        Task<User> GetUserOrCreateAsync(ulong userId);
        Task<User?> GetByShindenIdAsync(ulong userShindenId);
        Task<User?> GetByShindenIdAsync(ulong userShindenId, UserQueryOptions userQueryOptions);
        Task<User?> GetCachedFullUserAsync(ulong userId);
        Task<User?> GetCachedFullUserByShindenIdAsync(ulong userId);
        Task<List<User>> GetCachedAllUsersLiteAsync();
        Task<List<User>> GetCachedAllUsersAsync();
        Task SaveChangesAsync();
        Task<bool> ExistsByDiscordIdAsync(ulong userId);
        Task<User?> GetByDiscordIdAsync(ulong discordUserId);
        Task<bool> ExistsByShindenIdAsync(ulong id);
    }
}
