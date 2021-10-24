using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;

namespace Sanakan.DAL.Repositories
{
    internal class TransferAnalyticsRepository : BaseRepository<TransferAnalytics>, ITransferAnalyticsRepository
    {
        private readonly SanakanDbContext _dbContext;

        public TransferAnalyticsRepository(
            SanakanDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
