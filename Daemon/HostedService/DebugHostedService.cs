using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Daemon.HostedService
{
    /// <summary>
    /// Implements background service which does:
    /// assign existing users connected to Shinden to provided user role.
    /// invoke random action by bot.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class DebugHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITaskManager _taskManager;
        private readonly ILogger _logger;
        private readonly ITimer _timer;
        private readonly ISystemClock _systemClock;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IEnumerable<Action> _actions = Enum.GetValues<Action>();
        private readonly List<Action> _availableActions = Enum.GetValues<Action>().ToList();
        private readonly string _text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.Nunc semper, sem in hendrerit bibendum, lectus nunc aliquet lorem, at accumsan sem purus a neque.Sed in nisi vel eros viverra commodo.Curabitur ullamcorper sed dui eget efficitur. Pellentesque quam neque, pharetra eu malesuada eget, rhoncus ac justo.Vivamus in dignissim ex";
        private readonly Configuration _configuration;
        private DateTime? _expeditionStartedOn = null;
        private DateTime? _accessedMarketOn = null;
        private DateTime? _retrievedCardOn = null;
        private DateTime? _retrievedDailyCoinsOn = null;
        private DateTime? _invokedLotteryOn = null;
        private bool _isRunning = false;

        public DebugHostedService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<DebugHostedService> logger,
            ITaskManager taskManager,
            ITimer timer,
            ISystemClock systemClock,
            IRandomNumberGenerator randomNumberGenerator,
            IDiscordClientAccessor discordClientAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _taskManager = taskManager;
            _timer = timer;
            _systemClock = systemClock;
            _discordClientAccessor = discordClientAccessor;
            _randomNumberGenerator = randomNumberGenerator;
            _discordClientAccessor.Ready += ReadyAsync;
            _configuration = new Configuration();
        }

        private enum Action
        {
            SendMessage = 0,
            GetFreeCard = 1,
            GetDailyCoins = 2,
            GoToMarket = 3,
            BuyItem = 4,
            Duel = 5,
            StartExpedition = 6,
            EndExpedition = 7,
            Lottery = 8
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            try
            {
                var client = _discordClientAccessor.Client;
                var guild = await client.GetGuildAsync(_configuration.MainGuildId);
                var users = await guild.GetUsersAsync();

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
                var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
                var guildConfig = await guildConfigRepository.GetCachedById(guild.Id);
                var userRoleId = guildConfig.UserRoleId;

                if (userRoleId.HasValue)
                {
                    var databaseUsers = await userRepository.GetAllCachedAsync();

                    var combined = users.Join(databaseUsers, pr => pr.Id, pr => pr.Id, (src, dest) => (src, dest));

                    foreach (var (user, databaseUser) in combined)
                    {
                        if (!databaseUser.ShindenId.HasValue)
                        {
                            continue;
                        }

                        if (!user.RoleIds.Contains(userRoleId.Value))
                        {
                            await user.AddRoleAsync(userRoleId.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while setting user roles");
            }

            _timer.Tick += OnTick;
            _timer.Start(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        [SuppressMessage("Microsoft.Analyzers.ManagedCodeAnalysis", "CA1502:AvoidExcessiveComplexity", Justification = "Resolved")]
        private async void OnTick(object sender, TimerEventArgs eventArgs)
        {
            if (_isRunning)
            {
                return;
            }

            try
            {
                _isRunning = true;

                var action = _randomNumberGenerator.GetOneRandomFrom(_availableActions);
                var prefix = _configuration.Prefix;
                var client = _discordClientAccessor.Client;
                var channel = (ITextChannel)await client.GetChannelAsync(_configuration.MainChannelId);
                var utcNow = _systemClock.UtcNow;
                var bot = client.CurrentUser;

                using var serviceScope = _serviceScopeFactory.CreateScope();
                var serviceProvider = serviceScope.ServiceProvider;
                var userRepository = serviceProvider.GetRequiredService<IUserRepository>();

                var botUser = await userRepository.GetCachedAsync(bot.Id);
                Card? card;

                switch (action)
                {
                    case Action.GetDailyCoins:
                        if (_retrievedDailyCoinsOn.HasValue
                            && utcNow - _retrievedDailyCoinsOn.Value < TimeSpan.FromDays(1))
                        {
                            return;
                        }

                        await channel.SendMessageAsync($"{prefix}daily");
                        _retrievedDailyCoinsOn = utcNow;
                        break;
                    case Action.GetFreeCard:
                        if (_retrievedCardOn.HasValue
                           && utcNow - _retrievedCardOn.Value < TimeSpan.FromDays(1))
                        {
                            return;
                        }

                        await channel.SendMessageAsync($"{prefix}karta+");
                        _retrievedCardOn = utcNow;
                        break;
                    case Action.StartExpedition:
                        if (_expeditionStartedOn.HasValue)
                        {
                            if (utcNow - _expeditionStartedOn.Value > TimeSpan.FromHours(1))
                            {
                                card = botUser.GameDeck.Cards
                                    .FirstOrDefault(pr => pr.Expedition != ExpeditionCardType.None);

                                if (card == null)
                                {
                                    return;
                                }

                                await channel.SendMessageAsync($"{prefix}expedition end {card.Id}");

                                card = _randomNumberGenerator.GetOneRandomFrom(botUser.GameDeck.Cards);

                                await channel.SendMessageAsync($"{prefix}expedition {card.Id} normalna");
                                _expeditionStartedOn = utcNow;
                            }
                        }
                        else
                        {
                            card = _randomNumberGenerator.GetOneRandomFrom(botUser.GameDeck.Cards);

                            if (card == null)
                            {
                                return;
                            }

                            await channel.SendMessageAsync($"{prefix}expedition {card.Id} normalna");
                            _expeditionStartedOn = utcNow;
                        }

                        break;
                    case Action.EndExpedition:
                        if (_expeditionStartedOn.HasValue
                            && utcNow - _expeditionStartedOn.Value < TimeSpan.FromHours(1))
                        {
                            return;
                        }

                        card = botUser.GameDeck.Cards
                                 .FirstOrDefault(pr => pr.Expedition != ExpeditionCardType.None);

                        if (card == null)
                        {
                            return;
                        }

                        await channel.SendMessageAsync($"{prefix}expedition end {card.Id}");
                        _expeditionStartedOn = utcNow;
                        break;
                    case Action.GoToMarket:
                        if (_accessedMarketOn.HasValue
                           && utcNow - _accessedMarketOn.Value < TimeSpan.FromHours(1))
                        {
                            return;
                        }

                        card = _randomNumberGenerator.GetOneRandomFrom(botUser.GameDeck.Cards);
                        await channel.SendMessageAsync($"{prefix}market {card.Id}");
                        _accessedMarketOn = utcNow;
                        break;
                    case Action.BuyItem:
                        var item = _randomNumberGenerator.GetOneRandomFrom(Game.Constants.ItemsWithCostForActivityShop);
                        await channel.SendMessageAsync($"{prefix}shop {item.Index}");
                        break;
                    case Action.Duel:
                        await channel.SendMessageAsync($"{prefix}duel");
                        break;
                    case Action.SendMessage:
                        await channel.SendMessageAsync(_text);
                        break;
                    case Action.Lottery:
                        if (_invokedLotteryOn.HasValue
                           && utcNow - _invokedLotteryOn.Value < TimeSpan.FromMinutes(2))
                        {
                            return;
                        }

                        var delay = TimeSpan.FromSeconds(30);
                        await channel.SendMessageAsync($"{prefix}dev rozdaj 1 2 {delay}");
                        _invokedLotteryOn = utcNow;
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while invoking random bot action");
            }

            _isRunning = false;
        }

        private class Configuration
        {
            public string Prefix { get; set; } = ".";

            public ulong MainGuildId { get; set; } = 910284207098560584;

            public ulong MainChannelId { get; set; } = 910284207534796800;
        }
    }
}
