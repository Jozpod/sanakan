using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL
{
    public interface IDatabaseFacade
    {
        Task EnsureCreatedAsync(CancellationToken stoppingToken = default);
    }
}
