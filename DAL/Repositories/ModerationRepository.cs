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
        private readonly BuildDatabaseContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public ModerationRepository(
            BuildDatabaseContext dbContext,
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

        public Task<IEnumerable<PenaltyInfo>> GetCachedFullPenalties()
        {
            throw new NotImplementedException();
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
