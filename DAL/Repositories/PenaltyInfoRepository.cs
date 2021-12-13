using Microsoft.EntityFrameworkCore;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class PenaltyInfoRepository : BaseRepository<PenaltyInfo>, IPenaltyInfoRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public PenaltyInfoRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public Task<List<PenaltyInfo>> GetByGuildIdAsync(ulong discordGuildId)
        {
            return _dbContext.Penalties
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.GuildId == discordGuildId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties()
        {
            var cacheResult = _cacheManager.Get<IEnumerable<PenaltyInfo>>(CacheKeys.Penalties);

            if (cacheResult != null)
            {
                return cacheResult.Value!;
            }

            var result = await _dbContext
                .Penalties
                .AsQueryable()
                .Include(x => x.Roles)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync();

            _cacheManager.Add(CacheKeys.Penalties, result);

            return result;
        }

        public async Task<IEnumerable<PenaltyInfo>> GetMutedPenaltiesAsync(ulong discordGuildId)
        {
            var cacheResult = _cacheManager.Get<List<PenaltyInfo>>(CacheKeys.Muted);

            if (cacheResult != null)
            {
                return cacheResult.Value ?? Enumerable.Empty<PenaltyInfo>();
            }

            var list = await _dbContext
                .Penalties
                .Include(x => x.Roles)
                .Where(x => x.GuildId == discordGuildId
                    && x.Type == PenaltyType.Mute)
                .ToListAsync();

            _cacheManager.Add(CacheKeys.Muted, list);

            return list;
        }

        public Task<PenaltyInfo> GetPenaltyAsync(
            ulong discordUserId,
            ulong discordGuildId,
            PenaltyType penaltyType)
        {
            return _dbContext.Penalties
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.UserId == discordUserId
                    && x.Type == penaltyType
                    && x.GuildId == discordGuildId);

        }

        public async Task RemovePenaltyAsync(PenaltyInfo penalty)
        {
            _dbContext.OwnedRoles.RemoveRange(penalty.Roles);
            _dbContext.Penalties.Remove(penalty);

            await _dbContext.SaveChangesAsync();
        }
    }
}
