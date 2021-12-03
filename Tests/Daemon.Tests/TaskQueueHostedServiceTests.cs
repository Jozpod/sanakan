using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Daemon.Builder;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Builder;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests
{
    [TestClass]
    public class TaskQueueHostedServiceTests
    {
        private static IDatabaseFacade _databaseFacade;
        private static TaskQueueHostedService _service;
        private static IBlockingPriorityQueue _blockingPriorityQueue;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddSystemClock();
            serviceCollection.AddSingleton<ILogger<TaskQueueHostedService>>(NullLogger<TaskQueueHostedService>.Instance);
            serviceCollection.AddSingleton<TaskQueueHostedService>();
            serviceCollection.AddTaskQueue();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<DatabaseConfiguration>(configurationRoot.GetSection("Database"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            serviceCollection.AddDatabaseFacade();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _service = serviceProvider.GetRequiredService<TaskQueueHostedService>();
            _blockingPriorityQueue = serviceProvider.GetRequiredService<IBlockingPriorityQueue>();
            var sanakanDbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            _databaseFacade = serviceProvider.GetRequiredService<IDatabaseFacade>();

            await _databaseFacade.EnsureCreatedAsync();

            var user = new User(1, DateTime.UtcNow);
            sanakanDbContext.Users.Add(user);
            await sanakanDbContext.SaveChangesAsync();
        }

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await _databaseFacade.EnsureDeletedAsync();
        }

        [TestMethod]
        public async Task Should_Process_Messages()
        {
            _blockingPriorityQueue.TryEnqueue(new ConnectUserMessage()
            {
                DiscordUserId = 1ul,
                ShindenUserId = 2ul,
            });

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(5));
            await _service.StartAsync(cancellationTokenSource.Token);
        }
    }
}
