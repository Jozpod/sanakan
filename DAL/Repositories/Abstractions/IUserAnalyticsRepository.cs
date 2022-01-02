using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IUserAnalyticsRepository :
        ICreateRepository<UserAnalytics>, ISaveRepository
    {
    }
}
