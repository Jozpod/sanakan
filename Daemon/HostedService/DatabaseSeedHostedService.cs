using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sanakan.DAL;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    /// <summary>
    /// Implements background service for database seeding.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class DatabaseSeedHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DatabaseSeedHostedService(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var databaseSeeder = serviceScope.ServiceProvider.GetService<IDatabaseSeeder>();
            await databaseSeeder.RunAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
