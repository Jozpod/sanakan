using DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    internal class UserRepository : IUserRepository
    {
        public UserRepository()
        {

        }

        public static async Task<User> GetCachedFullUserAsync(this Database.UserContext context, ulong userId)
        {
            return (await context.Users.AsQueryable().Where(x => x.Id == userId).Include(x => x.Stats).Include(x => x.SMConfig).Include(x => x.TimeStatuses).Include(x => x.GameDeck).ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck).ThenInclude(x => x.Items).Include(x => x.GameDeck).ThenInclude(x => x.Cards).ThenInclude(x => x.ArenaStats).Include(x => x.GameDeck)
                .ThenInclude(x => x.BoosterPacks).ThenInclude(x => x.Characters).Include(x => x.GameDeck).ThenInclude(x => x.BoosterPacks).ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck).ThenInclude(x => x.ExpContainer).Include(x => x.GameDeck).ThenInclude(x => x.Wishes).Include(x => x.GameDeck).ThenInclude(x => x.Cards).ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck).ThenInclude(x => x.Figures).AsNoTracking().AsSplitQuery().FromCacheAsync(new string[] { $"user-{userId}", "users" })).FirstOrDefault();
        }

        public static async Task<User> GetCachedFullUserByShindenIdAsync(this Database.UserContext context, ulong userId)
        {
            return (await context.Users.AsQueryable().Where(x => x.Shinden == userId).Include(x => x.Stats).Include(x => x.SMConfig).Include(x => x.TimeStatuses).Include(x => x.GameDeck).ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck).ThenInclude(x => x.Items).Include(x => x.GameDeck).ThenInclude(x => x.Cards).ThenInclude(x => x.ArenaStats).Include(x => x.GameDeck)
                .ThenInclude(x => x.BoosterPacks).ThenInclude(x => x.Characters).Include(x => x.GameDeck).ThenInclude(x => x.BoosterPacks).ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck).ThenInclude(x => x.ExpContainer).Include(x => x.GameDeck).ThenInclude(x => x.Wishes).Include(x => x.GameDeck).ThenInclude(x => x.Cards).ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck).ThenInclude(x => x.Figures).AsNoTracking().AsSplitQuery().FromCacheAsync(new string[] { $"user-{userId}", "users" })).FirstOrDefault();
        }

        public static async Task<List<User>> GetCachedAllUsersLiteAsync(this Database.UserContext context)
        {
            return (await context.Users.AsQueryable().AsNoTracking().AsSplitQuery().FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) })).ToList();
        }


    }
}
