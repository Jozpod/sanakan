using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;

namespace Sanakan.DAL.Repositories
{
    public class SystemAnalyticsRepository : BaseRepository<SystemAnalytics>, ISystemAnalyticsRepository
    {
        private readonly BuildDatabaseContext _dbContext;

        public SystemAnalyticsRepository(
            BuildDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
