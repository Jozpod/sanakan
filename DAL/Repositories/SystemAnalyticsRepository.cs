using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;

namespace Sanakan.DAL.Repositories
{
    public class SystemAnalyticsRepository : BaseRepository<SystemAnalytics>, ISystemAnalyticsRepository
    {
        private readonly SanakanDbContext _dbContext;

        public SystemAnalyticsRepository(
            SanakanDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
