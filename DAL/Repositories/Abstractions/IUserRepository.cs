using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByShindenIdAsync(ulong userShindenId);
        Task<User?> GetCachedFullUserAsync(ulong userId);
        Task<User?> GetCachedFullUserByShindenIdAsync(ulong userId);
        Task<List<User>> GetCachedAllUsersLiteAsync();
        Task SaveChangesAsync();
    }
}
