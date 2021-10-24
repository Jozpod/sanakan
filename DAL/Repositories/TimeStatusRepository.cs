using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class TimeStatusRepository : BaseRepository<TimeStatus>, ITimeStatusRepository
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
                .Where(x => x.Guild == discordGuildId)
                .ToListAsync();
        }

        public async Task<List<TimeStatus>> GetBySubTypeAsync()
        {
            var result = await _dbContext
                .TimeStatuses
                .AsNoTracking()
                //.FromCache(new[] { "users" })
                .Where(x => x.Type == StatusType.Color
                    || x.Type == StatusType.Globals)
                .ToListAsync();

            return result;
        }
    }
}
