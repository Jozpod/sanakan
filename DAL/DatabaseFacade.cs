using Microsoft.Extensions.DependencyInjection;
using System;
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

        public async Task EnsureCreatedAsync(CancellationToken stoppingToken = default)
        {
            await _semaphore.WaitAsync(stoppingToken);

            if (_created)
            {
                return;
            }

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var sanakanDbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            var databaseFacade = sanakanDbContext.Database;
            await databaseFacade.EnsureCreatedAsync(stoppingToken);
            _created = true;

            _semaphore.Release();
        }
    }
}
