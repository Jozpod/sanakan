using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Configuration;

namespace Sanakan.Services
{
    public class Daemonizer
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(40);

        private CancellationTokenSource _cts { get; set; }
        private DiscordSocketClient _client { get; set; }
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;

        public Daemonizer(
            DiscordSocketClient client,
            ILogger<Daemonizer> logger,
            IOptionsMonitor<SanakanConfiguration> config)
        {
            _client = client;
            _logger = logger;
            _config = config;
            _cts = new CancellationTokenSource();

            _client.Connected += ConnectedAsync;
            _client.Disconnected += DisconnectedAsync;
        }

        private Task ConnectedAsync()
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();

            return Task.CompletedTask;
        }

        private Task DisconnectedAsync(Exception ex)
        {
            _ = Task.Delay(_timeout, _cts.Token).ContinueWith(async _ =>
            {
                await CheckStateAsync();
            });

            return Task.CompletedTask;
        }

        private async Task CheckStateAsync()
        {
            if (!_config.CurrentValue.Demonization)
            {
                return;
            }
            
            _logger.LogInformation("Disconnected! Running demonization check.");

            if (_client.ConnectionState == ConnectionState.Connected)
            {
                return;
            }

            var timeout = Task.Delay(_timeout);
            var connect = _client.StartAsync();
            var task = await Task.WhenAny(timeout, connect);

            if (task != timeout && connect.IsCompletedSuccessfully)
            {
                _logger.LogInformation("Reconnected!");
                return;
            }

            _logger.LogInformation("Timeout! Shutting down!");
            Environment.Exit(1);
        }
    }
}
