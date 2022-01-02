using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;

namespace Sanakan.DAL.Repositories
{
    internal class CommandsAnalyticsRepository :
        BaseRepository<CommandsAnalytics>, ICommandsAnalyticsRepository
    {
        private readonly SanakanDbContext _dbContext;

        public CommandsAnalyticsRepository(
            SanakanDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
