using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Services;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Resources;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Game.Models;
using Sanakan.DiscordBot.Abstractions;
using Humanizer;
using Sanakan.Common.Cache;
using Sanakan.DiscordBot.Session;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Zabawy"), RequireUserRole]
    public class FunModule : SanakanModuleBase
    {
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISystemClock _systemClock;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ISlotMachine _slotMachine;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public FunModule(
            ISessionManager session,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IRandomNumberGenerator randomNumberGenerator,
            ISlotMachine slotMachine,
            ITaskManager taskManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _sessionManager = session;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _randomNumberGenerator = randomNumberGenerator;
            _slotMachine = slotMachine;
            _taskManager = taskManager;
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
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var mention = Context.User.Mention;
            var timeStatuses = databaseUser.TimeStatuses;
            var timeStatus = timeStatuses.FirstOrDefault(x => x.Type == StatusType.Daily);
            var statusType = StatusType.Daily;

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
                var content = $"{mention} następne drobne możesz otrzymać dopiero za {remainingTimeFriendly}!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: content);
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

            await ReplyAsync(embed: $"{mention} łap drobne na waciki!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("chce muta", RunMode = RunMode.Async)]
        [Alias("mute me", "chce mute")]
        [Summary("odbierasz darmowego muta od bota - na serio i nawet nie proś o odmutowanie")]
        [Remarks("")]
        public async Task GiveMuteAsync()
        {
            var user = Context.User as IGuildUser;
            var guild = Context.Guild;

            if (user == null)
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config == null)
            {
                await ReplyAsync(embed: Strings.ServerNotConfigured.ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var notifChannel = await guild.GetChannelAsync(config.NotificationChannelId);
            var userRole = guild.GetRole(config.UserRoleId.Value);
            var muteRole = guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                await ReplyAsync(embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user.RoleIds.Contains(muteRole.Id))
            {
                await ReplyAsync(embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            const int daysInYear = 365;
            var duration = TimeSpan.FromDays(_randomNumberGenerator.GetRandomValue(daysInYear) + 1);

            var acceptPayload = new AcceptSession.AcceptSessionPayload
            {
                Bot = Context.Client.CurrentUser,
                NotifyChannel = (ITextChannel)notifChannel,
                MuteRole = muteRole,
                UserRole = userRole,
                User = user,
                Duration = duration,
            };

            var session = new AcceptSession(user.Id, _systemClock.UtcNow, acceptPayload);
            _sessionManager.Remove(session);

            var content = $"{user.Mention} na pewno chcesz muta?".ToEmbedMessage(EMType.Error).Build();
            var replyMessage = await ReplyAsync(embed: content);
            await replyMessage.AddReactionsAsync(new IEmote[] {
                Emojis.Checked,
                Emojis.DeclineEmote
            });

            acceptPayload.MessageId = replyMessage.Id;
            acceptPayload.Channel = replyMessage.Channel;

            _sessionManager.Add(session);
        }

        [Command("zaskórniaki")]
        [Alias("hourly", "zaskorniaki")]
        [Summary("upadłeś tak nisko, że prosisz o SC pod marketem")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveHourlyScAsync()
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var hourly = databaseUser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Hourly);
            var mention = Context.User.Mention;

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
                await ReplyAsync(embed: $"{mention} następne zaskórniaki możesz otrzymać dopiero za {remainingTime}"
                    .ToEmbedMessage(EMType.Error).Build());
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

            await ReplyAsync(embed: $"{mention} łap piątaka!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("wylosuj", RunMode = RunMode.Async)]
        [Alias("ofm", "one from many")]
        [Summary("bot losuje jedną rzecz z podanych opcji")]
        [Remarks("user1 user2 user3"), RequireCommandChannel]
        public async Task GetOneFromManyAsync(
            [Summary("opcje z których bot losuje")]params string[] options)
        {
            if (options.Count() < 2)
            {
                await ReplyAsync(embed: "Podano zbyt mało opcji do wylosowania.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var allOptions = _randomNumberGenerator.Shuffle(options).ToList();

            var delay = _randomNumberGenerator.GetRandomValue(100, 500);
            await _taskManager.Delay(TimeSpan.FromMilliseconds(delay));

            var content = $"{Emotes.PinkArrow} {_randomNumberGenerator.GetOneRandomFrom(allOptions)}"
                .ToEmbedMessage(EMType.Success).WithAuthor(new EmbedAuthorBuilder()
                .WithUser(Context.User))
                .Build();
            await ReplyAsync(embed: content);
        }

        [Command("rzut")]
        [Alias("beat", "toss")]
        [Summary("bot wykonuje rzut monetą, wygrywasz kwotę, o którą się założysz")]
        [Remarks("reszka 10"), RequireCommandChannel]
        public async Task TossCoinAsync(
            [Summary("strona monety (orzeł/reszka)")]CoinSide coinSide,
            [Summary("ilość SC")]int amount)
        {
            var mention = Context.User.Mention;

            if (amount <= 0)
            {
                await ReplyAsync(embed: $"{mention} na minusie?!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (databaseUser.ScCount < amount)
            {
                await ReplyAsync(embed: $"{mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            databaseUser.ScCount -= amount;
            var thrown = (CoinSide)_randomNumberGenerator.GetRandomValue(2);
            var embed = $"{mention} pudło! Obecnie posiadasz {databaseUser.ScCount} SC.".ToEmbedMessage(EMType.Error);

            databaseUser.Stats.Tail += (thrown == CoinSide.Tail) ? 1 : 0;
            databaseUser.Stats.Head += (thrown == CoinSide.Head) ? 1 : 0;

            if (thrown == coinSide)
            {
                ++databaseUser.Stats.Hit;
                databaseUser.ScCount += amount * 2;
                databaseUser.Stats.IncomeInSc += amount;
                embed = $"{mention} trafiony zatopiony! Obecnie posiadasz {databaseUser.ScCount} SC.".ToEmbedMessage(EMType.Success);
            }
            else
            {
                ++databaseUser.Stats.Misd;
                databaseUser.Stats.ScLost += amount;
                databaseUser.Stats.IncomeInSc -= amount;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: embed.Build());
            await Context.Channel.SendFileAsync(string.Format(Paths.CoinPicture, (int)thrown));
        }

        [Command("ustaw automat")]
        [Alias("set slot")]
        [Summary("ustawia automat")]
        [Remarks("info"), RequireCommandChannel]
        public async Task SlotMachineSettingsAsync([Summary("typ nastaw (info - wyświetla informacje)")]SlotMachineSetting setting = SlotMachineSetting.Info, [Summary("wartość nastawy")]string value = "info")
        {
            if (setting == SlotMachineSetting.Info)
            {
                var content = $"{Strings.SlotMachineInfo}".ToEmbedMessage(EMType.Info).Build();
                await ReplyAsync("", false, content);
                return;
            }

            var databaseUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (!databaseUser.ApplySlotMachineSetting(setting, value))
            {
                await ReplyAsync(embed: $"Podano niewłaściwą wartość parametru!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{Context.User.Mention} zmienił nastawy automatu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("automat")]
        [Alias("slot", "slot machine")]
        [Summary("grasz na jednorękim bandycie")]
        [Remarks("info"), RequireCommandChannel]
        public async Task PlayOnSlotMachineAsync(
            [Summary("typ (info - wyświetla informacje)")]string type = "game")
        {
            var info = string.Format(Strings.GameInfo, Emojis.PsyduckEmoji);

            foreach (var slotMachineSlot in SlotMachineSlotsExtensions.SlotMachineSlots)
            {
                if (slotMachineSlot != SlotMachineSlot.max
                        && slotMachineSlot != SlotMachineSlot.q)
                {
                    for (int i = 3; i < 6; i++)
                    {
                        string val = $"x{slotMachineSlot.WinValue(i)}";
                        info += $"{i}x{slotMachineSlot.Icon()} - {val.PadRight(5, ' ')} ";
                    }
                    info += "\n";
                }
            }

            if (type != "game")
            {
                await ReplyAsync("", false, $"{Strings.SlotMachineInfo}"
                    .ToEmbedMessage(EMType.Info).Build());
                return;
            }

            var discordUser = Context.User;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);

            var toPay = _slotMachine.ToPay(databaseUser);

            if (databaseUser.ScCount < toPay)
            {
                var content1 = $"{Context.User.Mention} brakuje Ci SC, aby za tyle zagrać.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: content1);
                return;
            }

            var win = _slotMachine.Play(databaseUser);
            databaseUser.ScCount += win - toPay;

            await _userRepository.SaveChangesAsync();

            var smConfig = databaseUser.SMConfig;

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var psay = smConfig.PsayMode > 0 ? $"{Emojis.PsyduckEmoji} " : " ";
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

            var content = slotMachineResult.ToEmbedMessage(EMType.Bot).Build();
            await ReplyAsync(embed: content);
        }

        [Command("podarujsc")]
        [Alias("donatesc")]
        [Summary("dajesz datek innemu graczowi w postaci SC obarczony 40% podatkiem")]
        [Remarks("Karna 2000"), RequireCommandChannel]
        public async Task GiveUserScAsync(
            [Summary("użytkownik")]IGuildUser user,
            [Summary("liczba SC (min. 1000)")]uint value)
        {
            if (value < 1000)
            {
                await ReplyAsync(embed: "Nie można podarować mniej niż 1000 SC.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var invokingUserId = Context.User.Id;
            var mention = Context.User.Mention;

            if (user.Id == invokingUserId)
            {
                await ReplyAsync(embed: "Coś tutaj nie gra.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (await _userRepository.ExistsByDiscordIdAsync(user.Id))
            {
                await ReplyAsync(embed: Strings.UserDoesNotExistInDatabase.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var targetUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var thisUser = await _userRepository.GetUserOrCreateAsync(invokingUserId);

            if (thisUser.ScCount < value)
            {
                await ReplyAsync(embed: $"{mention} nie masz wystarczającej ilości SC.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            thisUser.ScCount -= value;

            var newScCnt = (value * 60) / 100;
            targetUser.ScCount += newScCnt;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(thisUser.Id), CacheKeys.Users, CacheKeys.User(targetUser.Id));

            await ReplyAsync(embed: $"{mention} podarował {user.Mention} {newScCnt} SC".ToEmbedMessage(EMType.Success).Build());
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