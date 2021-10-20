using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ISystemAnalyticsRepository : 
        ICreateRepository<SystemAnalytics>, ISaveRepository
    {
    }
}
