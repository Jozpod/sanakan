using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.Daemon.HostedService;
using Sanakan.DAL;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Builder;
using Sanakan.DiscordBot.Services.Builder;
using Sanakan.DiscordBot.Session.Builder;
using Sanakan.Game.Builder;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Builder;
using Sanakan.TaskQueue.Builder;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
#if DEBUG
    [TestClass]
#endif
    public partial class TestBase
    {
        private static ServiceProvider _serviceProvider;
        private static IDiscordClientAccessor _discordClientAccessor;
        private static Mock<IShindenClient> _shindenClientMock = new();
        private static DiscordConfiguration _configuration;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private static IUserMessage? LastMessage = null;
        private static IGuild Guild;
        private static ITextChannel Channel;
        private static string Prefix = ".";
        private static IDatabaseFacade DatabaseFacade;
        private static DiscordIntegrationTestOptions _discordIntegrationTestOptions;
        private static TaskQueueHostedService _hostedService;
        private static CancellationToken _cancellationToken;
        private static SemaphoreSlim _guildAvailableSemaphore = new SemaphoreSlim(0);
        
        public static DiscordSocketClient FakeUserClient { get; private set; }
        public static SocketSelfUser FakeUser { get; private set; }

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddUserSecrets<DiscordIntegrationTestOptions>()
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
            services.AddDatabaseFacade();
            services.AddCache(configurationRoot.GetSection("Cache"));
            services.AddOperatingSystem();
            services.AddFileSystem();
            services.AddRandomNumberGenerator();
            services.AddResourceManager()
                .AddImageResources()
                .AddFontResources();

            services.AddSingleton(_shindenClientMock.Object);
            services.AddGameServices();
            services.AddDiscordBot();
            services.AddDiscordBotServices();
            services.Configure<DiscordIntegrationTestOptions>(configurationRoot.GetSection(nameof(DiscordIntegrationTestOptions)));
            services.AddConfiguration(configurationRoot);
            services.AddSingleton<IConfiguration>(configurationRoot);
            services.AddSingleton<TaskQueueHostedService>();
            _serviceProvider = services.BuildServiceProvider();

            _discordClientAccessor = _serviceProvider.GetRequiredService<IDiscordClientAccessor>();
            var discordConfiguration = _serviceProvider.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>();
            var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler>();
            var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
            _discordIntegrationTestOptions = _serviceProvider.GetRequiredService<IOptions<DiscordIntegrationTestOptions>>().Value;
            _hostedService = _serviceProvider.GetRequiredService<TaskQueueHostedService>();

            fileSystem.CreateDirectory(Paths.CardsMiniatures);
            fileSystem.CreateDirectory(Paths.CardsInProfiles);
            fileSystem.CreateDirectory(Paths.SavedData);
            fileSystem.CreateDirectory(Paths.Profiles);

            _configuration = discordConfiguration.CurrentValue;
            await _discordClientAccessor.LoginAsync(TokenType.Bot, _configuration.BotToken);
            await _discordClientAccessor.SetGameAsync($"{_configuration.Prefix}pomoc");
            await _discordClientAccessor.Client.StartAsync();

            await commandHandler.InitializeAsync();

            FakeUserClient = await SetupFakeUserBot();

            Guild = FakeUserClient.GetGuild(_configuration.MainGuild);
            Channel = await Guild.GetTextChannelAsync(_discordIntegrationTestOptions.MainChannelId);
            FakeUser = FakeUserClient.CurrentUser;
            FakeUserClient.MessageReceived += MessageReceivedAsync;
            _configuration.AllowedToDebug.Add(FakeUser.Id);

            DatabaseFacade = _serviceProvider.GetRequiredService<IDatabaseFacade>();
            if(await DatabaseFacade.EnsureCreatedAsync())
            {
                var dbContext = _serviceProvider.GetRequiredService<SanakanDbContext>();
                await TestDataGenerator.PopulateDatabaseAsync(
                    dbContext,
                    FakeUser.Id,
                    _discordIntegrationTestOptions);
            }

            _cancellationToken = new CancellationToken();
            _hostedService.StartAsync(_cancellationToken);
        }

       

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            if(DatabaseFacade != null)
            {
                //await DatabaseFacade.EnsureDeletedAsync();
            }
            if (_discordClientAccessor != null)
            {
                await _discordClientAccessor.LogoutAsync();
            }
            if (FakeUserClient != null)
            {
                await FakeUserClient.LogoutAsync();
            }

            if (_hostedService != null)
            {
                await _hostedService.StopAsync(_cancellationToken);
                _hostedService.Dispose();
            }
        }

        private static async Task<IUserMessage?> WaitForMessageAsync()
        {
            await _semaphore.WaitAsync(TimeSpan.FromHours(1));
            return LastMessage;
        }

        private static Task MessageReceivedAsync(IMessage message)
        {
            var userMessage = message as IUserMessage;

            if (userMessage == null)
            {
                return Task.CompletedTask;
            }

            if (message.Author.Id == FakeUser.Id)
            {
                return Task.CompletedTask;
            }

            LastMessage = userMessage;

            _semaphore.Release();
            return Task.CompletedTask;
        }

        public static async Task<DiscordSocketClient> SetupFakeUserBot()
        {
            var client = new DiscordSocketClient();
            client.GuildAvailable += GuildAvailable;
            await client.LoginAsync(TokenType.Bot, _discordIntegrationTestOptions.FakeUserBotToken);
            await client.StartAsync();

            await _guildAvailableSemaphore.WaitAsync();

            return client;
        }

        private static Task GuildAvailable(IGuild guild)
        {
            _guildAvailableSemaphore.Release();
            return Task.CompletedTask;
        }
    }
}
