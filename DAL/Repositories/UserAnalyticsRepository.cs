using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;

namespace Sanakan.DAL.Repositories
{
    public class UserAnalyticsRepository : BaseRepository<UserAnalytics>, IUserAnalyticsRepository
    {
        private readonly SanakanDbContext _dbContext;

        public UserAnalyticsRepository(
            SanakanDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
