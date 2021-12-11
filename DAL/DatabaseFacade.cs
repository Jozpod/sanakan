using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL
{
    public class DatabaseFacade : IDatabaseFacade
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private bool _created;

        public DatabaseFacade(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _created = false;
        }

        public async Task EnsureDeletedAsync(CancellationToken stoppingToken = default)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var sanakanDbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            var databaseFacade = sanakanDbContext.Database;
            await databaseFacade.EnsureDeletedAsync(stoppingToken);
        }

        public async Task<bool> EnsureCreatedAsync(CancellationToken stoppingToken = default)
        {
            await _semaphore.WaitAsync();

            if (_created)
            {
                _semaphore.Release();
                return _created;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var sanakanDbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            var databaseFacade = sanakanDbContext.Database;
            _created = await databaseFacade.EnsureCreatedAsync(stoppingToken).ConfigureAwait(true);

            _semaphore.Release();

            return _created;
        }
    }
}
