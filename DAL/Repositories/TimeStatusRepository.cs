using Microsoft.EntityFrameworkCore;
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
        private readonly BuildDatabaseContext _dbContext;

        public TimeStatusRepository(
            BuildDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<TimeStatus>> GetByGuildIdAsync(ulong discordGuildId)
        {
            return _dbContext.TimeStatuses
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Guild == discordGuildId)
                .ToListAsync();
        }
    }
}
