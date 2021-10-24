using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public UserRepository(
            SanakanDbContext dbContext,
            ISystemClock systemClock,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
        }

        public Task<List<ulong>> GetUserShindenIdsByHavingCharacterAsync(ulong id)
        {
            var result = _dbContext.Cards
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.User)
               .Where(x => x.CharacterId == id
                   && x.GameDeck.User.ShindenId.HasValue)
               .AsNoTracking()
               .Select(x => x.GameDeck.User.ShindenId.Value)
               .Distinct()
               .ToListAsync();

            return result;
        }

        public async Task<User?> GetUserWaifuProfileAsync(ulong shindenUserId)
        {
            var result = await _dbContext.Users
               .AsQueryable()
               .AsSplitQuery()
               .Where(x => x.ShindenId == shindenUserId)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.TagList)
               .AsNoTracking()
               .FirstOrDefaultAsync();

            return result;
        }
        public Task<User?> GetBaseUserAndDontTrackAsync(ulong discordUserId)
        {
            return _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == discordUserId);
        }

        public async Task<User> GetCachedFullUserAsync(ulong shindenUserId)
        {
            var key = $"user-{shindenUserId}";

            var cached = _cacheManager.Get<User>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext.Users
                .AsQueryable()
                .Where(x => x.Id == shindenUserId)
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.ArenaStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.Characters)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<User?> GetCachedFullUserByShindenIdAsync(ulong userId)
        {
            var key = $"user-{userId}";

            var cached = _cacheManager.Get<User>(key);

            if (cached == null)
            {
                return cached;
            }

            var result = await _dbContext.Users
                .AsQueryable()
                .Where(x => x.ShindenId == userId)
                .Include(x => x.Stats)
                .Include(x => x.SMConfig)
                .Include(x => x.TimeStatuses)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.PvPStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Items)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.ArenaStats)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.Characters)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.BoosterPacks)
                    .ThenInclude(x => x.RarityExcludedFromPack)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.ExpContainer)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Wishes)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Cards)
                    .ThenInclude(x => x.TagList)
                .Include(x => x.GameDeck)
                    .ThenInclude(x => x.Figures)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<List<User>> GetCachedAllUsersLiteAsync()
        {
            var key = "users-lite";

            var cached = _cacheManager.Get<List<User>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext.Users
                .AsQueryable()
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(key, result, new MemoryCacheEntryOptions { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });

            return result;
        }

        public Task<User?> GetByShindenIdAsync(ulong userShindenId)
        {
            var user = _dbContext.Users
                .FirstOrDefaultAsync(x => x.ShindenId == userShindenId);

            return user;
        }

        public async Task<User> GetUserOrCreateAsync(ulong discordUserId)
        {
            var user = await _dbContext.Users
               .AsQueryable()
               .Where(x => x.Id == discordUserId)
               .Include(x => x.Stats)
               .Include(x => x.SMConfig)
               .Include(x => x.TimeStatuses)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.PvPStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Wishes)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Items)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.ExpContainer)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.BoosterPacks)
               .ThenInclude(x => x.Characters)
                   .Include(x => x.GameDeck)
                   .ThenInclude(x => x.BoosterPacks)
                   .ThenInclude(x => x.RarityExcludedFromPack)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.TagList)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Figures)
               .AsSplitQuery()
               .FirstOrDefaultAsync();

                if (user == null)
                {
                    user = new User(discordUserId, _systemClock.StartOfMonth);
                    await _dbContext.Users.AddAsync(user);
                }

                return user;
        }

        public async Task<List<User>> GetCachedAllUsersAsync()
        {
            var key = "users";

            var cached = _cacheManager.Get<List<User>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext.Users
               .AsQueryable()
               .Include(x => x.Stats)
               .Include(x => x.SMConfig)
               .Include(x => x.TimeStatuses)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.PvPStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Wishes)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Items)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.ExpContainer)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.ArenaStats)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.BoosterPacks)
                   .ThenInclude(x => x.Characters)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.BoosterPacks)
                   .ThenInclude(x => x.RarityExcludedFromPack)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Cards)
                   .ThenInclude(x => x.TagList)
               .Include(x => x.GameDeck)
                   .ThenInclude(x => x.Figures)
               .AsNoTracking()
               .AsSplitQuery()
               .ToListAsync();

            _cacheManager.Add(key, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6)
            });

            return result;
        }

        public Task<bool> ExistsByDiscordIdAsync(ulong discordUserId)
            => _dbContext.Users.AnyAsync(x => x.Id == discordUserId);

        public Task<User?> GetByDiscordIdAsync(ulong discordUserId)
            => _dbContext.Users.FirstOrDefaultAsync(x => x.Id == discordUserId);

        public Task<User?> GetByShindenIdAsync(ulong userShindenId, UserQueryOptions userQueryOptions)
        {
            var query = _dbContext
                .Users
               .AsQueryable()
               .AsSplitQuery()
               .Where(x => x.ShindenId == userShindenId);

            if (userQueryOptions.IncludeGameDeck)
            {
                var includeQuery = query.Include(x => x.GameDeck);

                if (userQueryOptions.IncludeCards)
                {
                    var thenIncludeQuery = includeQuery.ThenInclude(x => x.Cards);
                    query = thenIncludeQuery;
                }
                else
                {
                    query = includeQuery;
                }

                if (userQueryOptions.IncludeWishes)
                {
                    var thenIncludeQuery = includeQuery.ThenInclude(x => x.Wishes);
                    query = thenIncludeQuery;
                }
                else
                {
                    query = includeQuery;
                }
            }

            return query
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public Task<bool> ExistsByShindenIdAsync(ulong userShindenId)
            => _dbContext.Users.AnyAsync(x => x.ShindenId == userShindenId);

        public Task<User> GetUserAndDontTrackAsync(ulong discordUserId)
        {
            var result = _dbContext
              .Users
              .AsQueryable()
              .Include(x => x.Stats)
              .Include(x => x.SMConfig)
              .Include(x => x.TimeStatuses)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.PvPStats)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.Wishes)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.Items)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.Cards)
                  .ThenInclude(x => x.ArenaStats)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.ExpContainer)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.BoosterPacks)
                  .ThenInclude(x => x.Characters)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.BoosterPacks)
                  .ThenInclude(x => x.RarityExcludedFromPack)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.Cards)
                  .ThenInclude(x => x.TagList)
              .Include(x => x.GameDeck)
                  .ThenInclude(x => x.Figures)
              .AsNoTracking()
              .AsSplitQuery()
              .FirstOrDefaultAsync(x => x.Id == discordUserId);

            return result;
        }

        public Task<List<User>> GetByShindenIdExcludeDiscordIdAsync(ulong shindenUserId, ulong discordUserId)
        {
            return _dbContext.Users
                .Where(x => x.ShindenId == shindenUserId
                    && x.Id != discordUserId)
                .ToListAsync();
        }

        public Task<List<ulong>> GetByExcludedDiscordIdsAsync(IEnumerable<ulong> discordUserIds)
        {
            return _dbContext.Users
                .AsSplitQuery()
                .Where(x => !discordUserIds.Any(id => id == x.Id))
                .Select(x => x.Id)
                .ToListAsync();

        }
    }
}
