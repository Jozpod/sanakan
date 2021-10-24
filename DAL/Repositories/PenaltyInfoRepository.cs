using Microsoft.EntityFrameworkCore;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class PenaltyInfoRepository : BaseRepository<PenaltyInfo>, IPenaltyInfoRepository
    {
        private readonly SanakanDbContext _dbContext;

        public PenaltyInfoRepository(
            SanakanDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<PenaltyInfo>> GetByGuildIdAsync(ulong discordGuildId)
        {
            return _dbContext.Penalties
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Guild == discordGuildId)
                .ToListAsync();
        }
    }
}
