using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL
{
    public interface IDatabaseFacade
    {
        Task EnsureDeletedAsync(CancellationToken stoppingToken = default);
        Task<bool> EnsureCreatedAsync(CancellationToken stoppingToken = default);
    }
}
