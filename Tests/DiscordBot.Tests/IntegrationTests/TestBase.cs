using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DiscordBot.Builder;
using Sanakan.DiscordBot.Services.Builder;
using Sanakan.DiscordBot.Session.Builder;
using Sanakan.Game.Builder;
using Sanakan.ShindenApi.Builder;
using Sanakan.TaskQueue.Builder;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    [TestClass]
    public partial class TestBase
    {
        private static ServiceProvider _serviceProvider;
        private static IDiscordClient _client;
        private static DiscordConfiguration _configuration;

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            services.AddOptions();
            services.AddWritableOption<SanakanConfiguration>();
            services.AddSystemClock();
            services.AddTaskQueue();
            services.AddSessionManager();
            services.AddTaskManager();
            services.AddSanakanDbContext(configurationRoot);
            services.AddRepositories();
            services.AddCache(configurationRoot.GetSection("Cache"));
            services.AddOperatingSystem();
            services.AddFileSystem();
            services.AddRandomNumberGenerator();
            services.AddResourceManager();
            services.AddImageResources();
            services.AddFontResources();
            services.AddShindenApi();
            services.AddGameServices();
            services.AddDiscordBot();
            services.AddDiscordBotServices();
            services.AddConfiguration(configurationRoot);
            services.AddSingleton<IConfiguration>(configurationRoot);
            _serviceProvider = services.BuildServiceProvider();

            var discordClientAccessor = _serviceProvider.GetRequiredService<IDiscordClientAccessor>();
            var discordConfiguration = _serviceProvider.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>();
            var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler>();

            _configuration = discordConfiguration.CurrentValue;
            await discordClientAccessor.LoginAsync(TokenType.Bot, _configuration.BotToken);
            await discordClientAccessor.SetGameAsync($"{_configuration.Prefix}pomoc");
            await discordClientAccessor.Client.StartAsync();

            await commandHandler.InitializeAsync();
            _client = discordClientAccessor.Client;
        }
    }
}
