using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ITransferAnalyticsRepository :
        ICreateRepository<TransferAnalytics>, ISaveRepository
    {
    }
}
