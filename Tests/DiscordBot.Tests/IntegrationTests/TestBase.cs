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
        public static DiscordSocketClient FakeUserClient { get; private set; }

        private static DiscordConfiguration _configuration;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private static IUserMessage? LastMessage = null;
        private static ulong ChannelId = 910284207534796800ul;

        private static IGuild Guild;

        private static ITextChannel Channel;
        private static string Prefix = ".";
        private static IDatabaseFacade DatabaseFacade;

        public static SocketSelfUser FakeUser { get; private set; }

        // 911409545094512671ul

        [ClassInitialize]
        public static async Task ClassInitialize(TestContext context)
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddUserSecrets<FakeUserBotOptions>()
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
            //services.AddShindenApi();

            services.AddSingleton(_shindenClientMock.Object);
            services.AddGameServices();
            services.AddDiscordBot();
            services.AddDiscordBotServices();
            services.Configure<FakeUserBotOptions>(configurationRoot.GetSection(nameof(FakeUserBotOptions)));
            services.AddConfiguration(configurationRoot);
            services.AddSingleton<IConfiguration>(configurationRoot);
            _serviceProvider = services.BuildServiceProvider();

            _discordClientAccessor = _serviceProvider.GetRequiredService<IDiscordClientAccessor>();
            var discordConfiguration = _serviceProvider.GetRequiredService<IOptionsMonitor<DiscordConfiguration>>();
            var commandHandler = _serviceProvider.GetRequiredService<ICommandHandler>();
            var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();

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
            await Task.Delay(2000);
            Guild = FakeUserClient.GetGuild(_configuration.MainGuild);
            Channel = await Guild.GetTextChannelAsync(ChannelId);
            FakeUser = FakeUserClient.CurrentUser;
            FakeUserClient.MessageReceived += MessageReceivedAsync;

            DatabaseFacade = _serviceProvider.GetRequiredService<IDatabaseFacade>();
            if(await DatabaseFacade.EnsureCreatedAsync())
            {
                var dbContext = _serviceProvider.GetRequiredService<SanakanDbContext>();
                await TestDataGenerator.PopulateDatabaseAsync(
                    dbContext,
                    _configuration.MainGuild,
                    FakeUser.Id,
                    ChannelId);
            }
        }

       

        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            if(DatabaseFacade != null)
            {
                await DatabaseFacade.EnsureCreatedAsync();
            }
            if (_discordClientAccessor != null)
            {
                await _discordClientAccessor.LogoutAsync();
            }
            if (FakeUserClient != null)
            {
                await FakeUserClient.LogoutAsync();
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
            var options = _serviceProvider.GetRequiredService<IOptions<FakeUserBotOptions>>().Value;
            var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, options.Token);
            await client.StartAsync();
            return client;
        }
    }
}
