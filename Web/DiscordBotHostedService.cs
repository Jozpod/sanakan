using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan
{
    public class DiscordBotHostedService : BackgroundService
    {
        private readonly SynchronizedExecutor _executor;
        private readonly ShindenClient _shindenClient;
        private readonly DiscordSocketClient _client;
        private readonly SessionManager _sessions;
        private readonly CommandHandler _handler;
        private readonly ExperienceManager _exp;
        private readonly Supervisor _supervisor;
        private readonly ImageProcessing _img;
        private readonly DeletedLog _deleted;
        private readonly Daemonizer _daemon;
        private readonly Greeting _greeting;
        private readonly Profile _profile;
        private readonly IConfig _config;
        private readonly ILogger _logger;
        private readonly Moderator _mod;
        private readonly Helper _helper;
        private readonly Events _events;
        private readonly Waifu _waifu;
        private readonly Spawn _spawn;
        private readonly Chaos _chaos;

        public DiscordBotHostedService()
        {

        }

        private void CreateModules()
        {
            Services.Dir.Create();

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200,
            });

            _client.Log += log =>
            {
                _logger.Log(log.ToString());
                return Task.CompletedTask;
            };

            var tmpCnf = _config.Get();
            _shindenClient = new ShindenClient(new Auth(tmpCnf.Shinden.Token,
                tmpCnf.Shinden.UserAgent, tmpCnf.Shinden.Marmolade), _logger);

            _helper = new Helper(_config);
            _events = new Events(_shindenClient);
            _img = new ImageProcessing(_shindenClient);
            _deleted = new DeletedLog(_client, _config);
            _chaos = new Chaos(_client, _config, _logger);
            _executor = new SynchronizedExecutor(_logger);
            _mod = new Moderator(_logger, _config, _client);
            _waifu = new Waifu(_img, _shindenClient, _events);
            _daemon = new Daemonizer(_client, _logger, _config);
            _sessions = new SessionManager(_client, _executor, _logger);
            _supervisor = new Supervisor(_client, _config, _logger, _mod);
            _greeting = new Greeting(_client, _logger, _config, _executor);
            _exp = new ExperienceManager(_client, _executor, _config, _img);
            _spawn = new Spawn(_client, _executor, _waifu, _config, _logger);
            _handler = new CommandHandler(_client, _config, _logger, _executor);
            _profile = new Profile(_client, _shindenClient, _img, _logger, _config);
        }

        private void LoadConfig()
        {
#if !DEBUG
            _config = new ConfigManager("Config.json");
#else
            _config = new ConfigManager("ConfigDebug.json");
#endif
        }

        private IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
               
                .BuildServiceProvider();
        }

        private void AddSigTermHandler()
        {
            Console.CancelKeyPress += delegate
            {
                _ = Task.Run(async () =>
                {
                    _logger.Log("SIGTERM Received!");
                    await _client.LogoutAsync();
                    await Task.Delay(1000);
                    Environment.Exit(0);
                });
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            LoadConfig();
            CreateModules();
            AddSigTermHandler();

            using (var db = new Database.BuildDatabaseContext(_config))
            {
                db.Database.EnsureCreated();
            }

            var tmpCnf = _config.Get();
            await _client.LoginAsync(TokenType.Bot, tmpCnf.BotToken);
            await _client.SetGameAsync(tmpCnf.Prefix + "pomoc");
            await _client.StartAsync();

            var services = BuildServiceProvider();

            _executor.Initialize(services);
            _sessions.Initialize(services);
            await _handler.InitializeAsync(services, _helper);

            await Task.Delay(-1);
        }
    }
}
