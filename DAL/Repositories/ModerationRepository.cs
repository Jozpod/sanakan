using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class ModerationRepository : IModerationRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public ModerationRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public async Task AddPenaltyAsync(PenaltyInfo info)
        {
            await _dbContext.Penalties.AddAsync(info);
            await _dbContext.SaveChangesAsync();
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

        public async Task<List<PenaltyInfo>> GetMutedPenaltiesAsync(ulong discordGuildId)
        {
            var list = await _dbContext
                .Penalties
                .Include(x => x.Roles)
               .Where(x => x.Guild == discordGuildId
                   && x.Type == PenaltyType.Mute)
               .ToListAsync();

            //.FromCacheAsync(new string[] { $"mute" }))

            return list;
        }

        public Task<PenaltyInfo> GetPenaltyAsync(
            ulong discordUserId,
            ulong discordGuildId,
            PenaltyType penaltyType)
        {
            return _dbContext.Penalties
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.User == discordUserId
                    && x.Type == penaltyType
                    && x.Guild == discordGuildId);

        }

        public async Task RemovePenaltyAsync(PenaltyInfo penalty)
        {
            _dbContext.OwnedRoles.RemoveRange(penalty.Roles);
            _dbContext.Penalties.Remove(penalty);

            await _dbContext.SaveChangesAsync();
        }
    }
}
