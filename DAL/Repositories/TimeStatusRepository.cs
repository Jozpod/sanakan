using Microsoft.EntityFrameworkCore;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    internal class TimeStatusRepository : BaseRepository<TimeStatus>, ITimeStatusRepository
    {
        private readonly SanakanDbContext _dbContext;
        private readonly ICacheManager _cacheManager;

        public TimeStatusRepository(
            SanakanDbContext dbContext,
            ICacheManager cacheManager) : base(dbContext)
        {
            _dbContext = dbContext;
            _cacheManager = cacheManager;
        }

        public Task<List<TimeStatus>> GetByGuildIdAsync(ulong discordGuildId)
        {
            return _dbContext.TimeStatuses
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.GuildId == discordGuildId)
                .ToListAsync();
        }

        public async Task<List<TimeStatus>> GetBySubTypeAsync()
        {
            var cacheResult = _cacheManager.Get<List<TimeStatus>>(CacheKeys.TimeStatuses);

            if (cacheResult != null)
            {
                return cacheResult.Value!;
            }

            var result = await _dbContext
                .TimeStatuses
                .AsNoTracking()
                .Where(x => x.Type == StatusType.Color
                    || x.Type == StatusType.Globals)
                .ToListAsync();

            _cacheManager.Add(CacheKeys.TimeStatuses, result, CacheKeys.Users);

            return result;
        }
    }
}
