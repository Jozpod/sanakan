using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;

namespace Sanakan.DAL.Repositories
{
    public class UserAnalyticsRepository : BaseRepository<UserAnalytics>, IUserAnalyticsRepository
    {
        private readonly BuildDatabaseContext _dbContext;

        public UserAnalyticsRepository(
            BuildDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
