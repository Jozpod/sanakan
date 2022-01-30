using Discord;
using Discord.Commands;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Session;
using Sanakan.DiscordBot.Session.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Zabawy"), RequireUserRole]
    public class FunModule : SanakanModuleBase
    {
        private readonly IIconConfiguration _iconConfiguration;
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISystemClock _systemClock;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ISlotMachine _slotMachine;
        private readonly ITaskManager _taskManager;
        private readonly IFileSystem _fileSystem;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public FunModule(
            IIconConfiguration iconConfiguration,
            ISessionManager sessionManager,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IRandomNumberGenerator randomNumberGenerator,
            ISlotMachine slotMachine,
            ITaskManager taskManager,
            IFileSystem fileSystem,
            IServiceScopeFactory serviceScopeFactory)
        {
            _iconConfiguration = iconConfiguration;
            _sessionManager = sessionManager;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _randomNumberGenerator = randomNumberGenerator;
            _slotMachine = slotMachine;
            _taskManager = taskManager;
            _fileSystem = fileSystem;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _questionRepository = serviceProvider.GetRequiredService<IQuestionRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("drobne")]
        [Alias("daily")]
        [Summary("dodaje dzienną dawkę drobniaków do twojego portfela")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveDailyScAsync()
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var mention = user.Mention;
            var timeStatuses = databaseUser.TimeStatuses;
            var timeStatus = timeStatuses.FirstOrDefault(x => x.Type == StatusType.Daily);
            var statusType = StatusType.Daily;
            Embed embed;

            if (timeStatus == null)
            {
                timeStatus = new TimeStatus(statusType);
                databaseUser.TimeStatuses.Add(timeStatus);
            }

            var utcNow = _systemClock.UtcNow;

            if (timeStatus.IsActive(utcNow))
            {
                var remainingTime = timeStatus.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                embed = $"{mention} następne drobne możesz otrzymać dopiero za {remainingTimeFriendly}!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            statusType = StatusType.WDaily;
            var mission = timeStatuses
                .FirstOrDefault(x => x.Type == statusType);

            if (mission == null)
            {
                mission = new TimeStatus(statusType);
                databaseUser.TimeStatuses.Add(mission);
            }

            timeStatus.EndsOn = utcNow.AddHours(20);
            databaseUser.ScCount += 100;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id));

            embed = $"{mention} łap drobne na waciki!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("chce muta", RunMode = RunMode.Async)]
        [Alias("mute me", "chce mute")]
        [Summary("odbierasz darmowego muta od bota - na serio i nawet nie proś o odmutowanie")]
        [Remarks("")]
        public async Task GiveMuteAsync()
        {
            var user = Context.User as IGuildUser;
            var guild = Context.Guild;
            Embed embed;

            if (user == null)
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedById(guild.Id);

            if (config == null)
            {
                await ReplyAsync(embed: Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var notifyChannel = (ITextChannel)await guild.GetChannelAsync(config.NotificationChannelId);
            var userRole = guild.GetRole(config.UserRoleId!.Value);
            var muteRole = guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                embed = Strings.MuteRoleNotSet.ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (user.RoleIds.Contains(muteRole.Id))
            {
                embed = $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            _sessionManager.RemoveIfExists<AcceptSession>(user.Id);

            embed = $"{user.Mention} na pewno chcesz muta?".ToEmbedMessage(EMType.Error).Build();
            var replyMessage = await ReplyAsync(embed: embed);
            await replyMessage.AddReactionsAsync(_iconConfiguration.AcceptDecline);

            var session = new AcceptSession(
                user.Id,
                _systemClock.UtcNow,
                Context.Client.CurrentUser,
                user,
                replyMessage,
                replyMessage.Channel,
                notifyChannel,
                userRole,
                muteRole);

            _sessionManager.Add(session);
        }

        [Command("zaskórniaki")]
        [Alias("hourly", "zaskorniaki")]
        [Summary("upadłeś tak nisko, że prosisz o SC pod marketem")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveHourlyScAsync()
        {
            var user = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var hourly = databaseUser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Hourly);
            var mention = user.Mention;
            Embed embed;

            if (hourly == null)
            {
                hourly = new TimeStatus(StatusType.Hourly);
                databaseUser.TimeStatuses.Add(hourly);
            }

            var utcNow = _systemClock.UtcNow;

            if (hourly.IsActive(utcNow))
            {
                var remainingTime = hourly.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                embed = $"{mention} następne zaskórniaki możesz otrzymać dopiero za {remainingTime}"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            hourly.EndsOn = utcNow.AddHours(1);
            databaseUser.ScCount += 5;

            var mission = databaseUser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.DHourly);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DHourly);
                databaseUser.TimeStatuses.Add(mission);
            }

            mission.Count(utcNow);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{mention} łap piątaka!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("wylosuj", RunMode = RunMode.Async)]
        [Alias("ofm", "one from many")]
        [Summary("bot losuje jedną rzecz z podanych opcji")]
        [Remarks("user1 user2 user3"), RequireCommandChannel]
        public async Task GetOneFromManyAsync(
            [Summary("opcje z których bot losuje")] params string[] options)
        {
            Embed embed;

            if (options.Count() < 2)
            {
                embed = "Podano zbyt mało opcji do wylosowania.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var allOptions = _randomNumberGenerator.Shuffle(options).ToList();

            var delay = _randomNumberGenerator.GetRandomValue(100, 500);
            await _taskManager.Delay(TimeSpan.FromMilliseconds(delay));

            embed = $"{Emotes.PinkArrow} {_randomNumberGenerator.GetOneRandomFrom(allOptions)}"
                .ToEmbedMessage(EMType.Success).WithAuthor(new EmbedAuthorBuilder()
                .WithUser(Context.User))
                .Build();
            await ReplyAsync(embed: embed);
        }

        [Command("rzut")]
        [Alias("beat", "toss")]
        [Summary("bot wykonuje rzut monetą, wygrywasz kwotę, o którą się założysz")]
        [Remarks("reszka 10"), RequireCommandChannel]
        public async Task TossCoinAsync(
            [Summary("strona monety (orzeł/reszka)")] CoinSide coinSide,
            [Summary("ilość SC")] int amount)
        {
            var mention = Context.User.Mention;
            Embed embed;

            if (amount <= 0)
            {
                embed = $"{mention} na minusie?!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (databaseUser.ScCount < amount)
            {
                embed = $"{mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            databaseUser.ScCount -= amount;
            var thrown = (CoinSide)_randomNumberGenerator.GetRandomValue(2);
            embed = $"{mention} pudło! Obecnie posiadasz {databaseUser.ScCount} SC.".ToEmbedMessage(EMType.Error).Build();

            var stats = databaseUser.Stats;
            if(thrown == CoinSide.Tail)
            {
                stats.Tail++;
            }
            else
            {
                stats.Head++;
            }

            if (thrown == coinSide)
            {
                ++stats.Hit;
                databaseUser.ScCount += amount * 2;
                stats.IncomeInSc += amount;
                embed = $"{mention} trafiony zatopiony! Obecnie posiadasz {databaseUser.ScCount} SC.".ToEmbedMessage(EMType.Success).Build();
            }
            else
            {
                ++stats.Misd;
                stats.ScLost += amount;
                stats.IncomeInSc -= amount;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: embed);
            var filePath = string.Format(Paths.CoinPicture, (int)thrown);
            using var stream = _fileSystem.OpenRead(filePath);
            await Context.Channel.SendFileAsync(stream, "coin.png");
        }

        [Command("ustaw automat")]
        [Alias("set slot")]
        [Summary("ustawia automat")]
        [Remarks("info"), RequireCommandChannel]
        public async Task SlotMachineSettingsAsync(
            [Summary("typ nastaw (info - wyświetla informacje)")] SlotMachineSetting setting = SlotMachineSetting.Info,
            [Summary("wartość nastawy")] string value = "info")
        {
            var user = Context.User;
            Embed embed;

            if (setting == SlotMachineSetting.Info)
            {
                var content = $"{Strings.SlotMachineInfo}".ToEmbedMessage(EMType.Info).Build();
                await ReplyAsync("", false, content);
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            if (!databaseUser!.ApplySlotMachineSetting(setting, value))
            {
                embed = $"Podano niewłaściwą wartość parametru!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            embed = $"{user.Mention} zmienił nastawy automatu.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("automat")]
        [Alias("slot", "slot machine")]
        [Summary("grasz na jednorękim bandycie")]
        [Remarks("info"), RequireCommandChannel]
        public async Task PlayOnSlotMachineAsync(
            [Summary("typ (info - wyświetla informacje)")] string type = "game")
        {
            var stringBuilder = new StringBuilder(string.Format(Strings.GameInfo, _iconConfiguration.Psyduck));

            Embed embed;

            foreach (var slotMachineSlot in SlotMachineSlotsExtensions.SlotMachineSlots)
            {
                if (slotMachineSlot != SlotMachineSlot.max
                        && slotMachineSlot != SlotMachineSlot.q)
                {
                    for (var index = 3; index < 6; index++)
                    {
                        var val = $"x{slotMachineSlot.WinValue(index)}".PadRight(5, ' ');
                        stringBuilder.AppendFormat(
                            "{0}x{1} - {2} ",
                            index,
                            slotMachineSlot.Icon(),
                            val);
                    }

                    stringBuilder.AppendLine();
                }
            }

            if (type != "game")
            {
                embed = Strings.SlotMachineInfo
                    .ToEmbedMessage(EMType.Info)
                    .Build();
                await ReplyAsync("", false, embed);
                return;
            }

            var discordUser = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);
            var smConfig = databaseUser.SMConfig;

            var toPay = _slotMachine.ToPay(smConfig);

            if (databaseUser.ScCount < toPay)
            {
                embed = $"{discordUser.Mention} brakuje Ci SC, aby za tyle zagrać.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var win = _slotMachine.Play(databaseUser);
            databaseUser.ScCount += win - toPay;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var psay = smConfig.PsayMode > 0 ? $"{_iconConfiguration.Psyduck} " : " ";
            var beatValue = smConfig.Beat.Value();
            var multiplierValue = smConfig.Multiplier.Value();

            var slotMachineResult = string.Format(
                Strings.SlotMachineResult,
                psay,
                discordUser.Mention,
                _slotMachine.Draw(databaseUser),
                beatValue,
                multiplierValue,
                win);

            embed = slotMachineResult.ToEmbedMessage(EMType.Bot).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("podarujsc")]
        [Alias("donatesc")]
        [Summary("dajesz datek innemu graczowi w postaci SC obarczony 40% podatkiem")]
        [Remarks("Karna 2000"), RequireCommandChannel]
        public async Task GiveUserScAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba SC (min. 1000)")] uint value)
        {
            Embed embed;

            if (value < 1000)
            {
                embed = "Nie można podarować mniej niż 1000 SC.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var invokingUser = Context.User;
            var invokingUserId = invokingUser.Id;
            var mention = invokingUser.Mention;

            if (user.Id == invokingUserId)
            {
                embed = "Coś tutaj nie gra.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (!await _userRepository.ExistsByDiscordIdAsync(user.Id))
            {
                await ReplyAsync(embed: Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var targetUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var thisUser = await _userRepository.GetUserOrCreateAsync(invokingUserId);

            if (thisUser.ScCount < value)
            {
                embed = $"{mention} nie masz wystarczającej ilości SC.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            thisUser.ScCount -= value;

            var newScCnt = (value * 60) / 100;
            targetUser.ScCount += newScCnt;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(thisUser.Id), CacheKeys.Users, CacheKeys.User(targetUser.Id));

            embed = $"{mention} podarował {user.Mention} {newScCnt} SC".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("zagadka", RunMode = RunMode.Async)]
        [Alias("riddle")]
        [Summary("wypisuje losową zagadkę i podaje odpowiedź po 15 sekundach")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowRiddleAsync()
        {
            var userId = Context.User.Id;
            var mention = Context.User.Mention;

            var riddles = new List<Question>();
            riddles = await _questionRepository.GetCachedAllQuestionsAsync();

            riddles = _randomNumberGenerator.Shuffle(riddles).ToList();
            var riddle = riddles.FirstOrDefault();

            riddle.RandomizeAnswers(_randomNumberGenerator);
            var message = await ReplyAsync(riddle.Get());
            await message.AddReactionsAsync(riddle.GetEmotes());

            await _taskManager.Delay(TimeSpan.FromSeconds(15));

            int answers = 0;
            var react = await message.GetReactionUsersAsync(riddle.GetRightEmote(), 100).FlattenAsync();
            foreach (var addReactions in riddle.GetEmotes())
            {
                var reactionUsers = await message.GetReactionUsersAsync(addReactions, 100).FlattenAsync();
                if (reactionUsers.Any(x => x.Id == userId))
                {
                    answers++;
                }
            }

            await message.RemoveAllReactionsAsync();

            if (react.Any(x => x.Id == userId) && answers < 2)
            {
                await ReplyAsync("", false, $"{mention} zgadłeś!"
                    .ToEmbedMessage(EMType.Success).Build());
            }
            else if (answers > 1)
            {
                await ReplyAsync("", false, $"{mention} wybrałeś więcej jak jedną odpowiedź!"
                    .ToEmbedMessage(EMType.Error).Build());
            }
            else
            {
                await ReplyAsync("", false, $"{mention} pudło!"
                    .ToEmbedMessage(EMType.Error).Build());
            }
        }
    }
}