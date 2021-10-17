﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Services.Commands;
using Sanakan.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Sanakan.Web.HostedService
{
    public class DiscordBotHostedService : BackgroundService
    {
        //private readonly SynchronizedExecutor _executor;
        //private readonly ShindenClient _shindenClient;
        private DiscordSocketClient _client;
        //private readonly SessionManager _sessions;
        private readonly CommandHandler _handler;
        //private readonly ExperienceManager _exp;
        //private readonly Supervisor _supervisor;
        //private readonly ImageProcessing _img;
        //private readonly DeletedLog _deleted;
        //private readonly Daemonizer _daemon;
        //private readonly Greeting _greeting;
        //private readonly Profile _profile;
        //private readonly IConfig _config;
        //private readonly Moderator _mod;
        //private readonly Helper _helper;
        //private readonly Events _events;
        //private readonly Waifu _waifu;
        //private readonly Spawn _spawn;
        //private readonly Chaos _chaos;
        private readonly ILogger _logger;

        private readonly IFileSystem _fileSystem;
        private readonly IServiceProvider _serviceProvider;
        
        public DiscordBotHostedService(
            IFileSystem fileSystem,
            ILogger<DiscordBotHostedService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _fileSystem = fileSystem;
        }

        private void CreateModules()
        {
            //_helper = new Helper(_config);
            //_events = new Events(_shindenClient);
            //_img = new ImageProcessing(_shindenClient);
            //_deleted = new DeletedLog(_client, _config);
            //_chaos = new Chaos(_client, _config, _logger);
            //_executor = new SynchronizedExecutor(_logger);
            //_mod = new Moderator(_logger, _config, _client);
            //_waifu = new Waifu(_img, _shindenClient, _events);
            //_daemon = new Daemonizer(_client, _logger, _config);
            //_sessions = new SessionManager(_client, _executor, _logger);
            //_supervisor = new Supervisor(_client, _config, _logger, _mod);
            //_greeting = new Greeting(_client, _logger, _config, _executor);
            //_exp = new ExperienceManager(_client, _executor, _config, _img);
            //_spawn = new Spawn(_client, _executor, _waifu, _config, _logger);
            //_handler = new CommandHandler(_client, _config, _logger, _executor);
            //_profile = new Profile(_client, _shindenClient, _img, _logger, _config);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var database = _serviceProvider.GetRequiredService<DatabaseFacade>();
            var configuration = _serviceProvider.GetRequiredService<IOptions<SanakanConfiguration>>().Value;
            await database.EnsureCreatedAsync();

            _fileSystem.CreateDirectory(Paths.CardsMiniatures);
            _fileSystem.CreateDirectory(Paths.CardsInProfiles);
            _fileSystem.CreateDirectory(Paths.SavedData);
            _fileSystem.CreateDirectory(Paths.Profiles);

            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>(); 
            _client.Log += onLog;

            await _client.LoginAsync(TokenType.Bot, configuration.BotToken);
            await _client.SetGameAsync(configuration.Prefix + "pomoc");
            await _client.StartAsync();

            //_executor.Initialize(services);
            //_sessions.Initialize(services);
            await _handler.InitializeAsync(services, _helper);

            await Task.Delay(Timeout.Infinite);
        }

        private Task onLog(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Debug:
                    _logger.LogDebug(log.ToString());
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(log.ToString());
                    break;
                case LogSeverity.Verbose:
                    _logger.LogDebug(log.ToString());
                    break;
                case LogSeverity.Error:
                    _logger.LogError(log.ToString());
                    break;
                default:
                    _logger.LogInformation(log.ToString());
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
