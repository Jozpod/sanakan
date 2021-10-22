using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class AllRepository : IAllRepository
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public AllRepository(
            BuildDatabaseContext dbContext,
            ISystemClock _systemClock,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task<GuildOptions> GetGuildConfigOrCreateAsync(ulong guildId)
        {
            var config = await _dbContext.Guilds
                .AsQueryable()
                .Include(x => x.IgnoredChannels)
                .Include(x => x.ChannelsWithoutExp)
                .Include(x => x.ChannelsWithoutSupervision)
                .Include(x => x.CommandChannels)
                .Include(x => x.SelfRoles)
                .Include(x => x.Lands)
                .Include(x => x.ModeratorRoles)
                .Include(x => x.RolesPerLevel)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.CommandChannels)
                .Include(x => x.Raports)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.FightChannels)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == guildId);

            if (config == null)
            {
                config = new GuildOptions
                {
                    Id = guildId,
                    SafariLimit = 50
                };

                await _dbContext.Guilds.AddAsync(config);
            }

            return config;
        }

        public async Task<GuildOptions> GetCachedGuildFullConfigAsync(ulong guildId)
        {
            var key = $"config-{guildId}";

            var cached = _cacheManager.Get<GuildOptions>(key);

            if(cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Guilds
                .AsQueryable()
                .Include(x => x.IgnoredChannels)
                .Include(x => x.ChannelsWithoutExp)
                .Include(x => x.ChannelsWithoutSupervision)
                .Include(x => x.CommandChannels)
                .Include(x => x.SelfRoles)
                .Include(x => x.Lands)
                .Include(x => x.ModeratorRoles)
                .Include(x => x.RolesPerLevel)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.CommandChannels)
                    .Include(x => x.Raports)
                .Include(x => x.WaifuConfig)
                    .ThenInclude(x => x.FightChannels)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == guildId);

            return result;
        }

        public async Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties()
        {
            var key = $"mute";

            var cached = _cacheManager.Get<IEnumerable<PenaltyInfo>>(key);

            if (cached != null)
            {
                return cached;
            }

            var result = await _dbContext
                .Penalties
                .AsQueryable()
                .Include(x => x.Roles)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(key, result);

            return result;
        }

        public async Task<List<User>> GetCachedAllUsersAsync()
        {
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
                //.FromCacheAsync(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) }))
                .ToListAsync();

            return result;
        }

        public async Task<User> GetUserAndDontTrackAsync(ulong userId)
        {
            var result = await _dbContext
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
                .FirstOrDefaultAsync(x => x.Id == userId);

            return result;
        }

        public async Task<Card> GetCardAsync(ulong wid)
        {
            var result = await _dbContext
                .Cards
                .Include(x => x.GameDeck)
                .Include(x => x.ArenaStats)
                .Include(x => x.TagList)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == wid);

            return result;
        }
        public async Task<Question> GetQuestionAsync(ulong id)
        {
            var result = await _dbContext
                .Questions
                .Include(x => x.Answer)
                .FirstOrDefaultAsync(x => x.Id == id);

            return result;
        }

        public async Task<User> GetUsersCardsByShindenIdWithOffsetAndFilterAsync1(ulong id)
        {
            var result = await _dbContext.Users.AsQueryable()
                .Where(x => x.Shinden == id)
                .Include(x => x.GameDeck)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<Card>> GetUsersCardsByShindenIdWithOffsetAndFilterAsync2(ulong id)
        {
            var result = await _dbContext.Cards
               .AsQueryable()
               .AsSplitQuery()
               .Where(x => x.GameDeckId == id)
               .Include(x => x.ArenaStats)
               .Include(x => x.TagList)
               .AsNoTracking()
               .ToListAsync();

            return result;
        }
    }
}
