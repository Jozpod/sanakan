using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.Session.Models;
using Sanakan.Services.Session;
using Sanakan.Services.SlotMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Services;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.TaskQueue;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Game.Models;
using Sanakan.DiscordBot.Abstractions;
using Humanizer;

namespace Sanakan.Modules
{
    [Name("Zabawy"), RequireUserRole]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        public const string PsyduckEmoji = "<:klasycznypsaj:482136878120828938>";
        private readonly IModeratorService _moderatorService;
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISystemClock _systemClock;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SlotMachine _slotMachine;
        public FunModule(
            IModeratorService moderatorService,
            ISessionManager session,
            ICacheManager cacheManager,
            ISystemClock systemClock,
            IServiceScopeFactory serviceScopeFactory)
        {
            _sessionManager = session;
            _moderatorService = moderatorService;
            _cacheManager = cacheManager;
            _systemClock = systemClock;
            _serviceScopeFactory = serviceScopeFactory;
        }

        [Command("drobne")]
        [Alias("daily")]
        [Summary("dodaje dzienną dawkę drobniaków do twojego portfela")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveDailyScAsync()
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            
            var timeStatus = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Daily);
            
            if (timeStatus == null)
            {
                timeStatus = new TimeStatus(StatusType.Daily);
                botuser.TimeStatuses.Add(timeStatus);
            }

            var utcNow = _systemClock.UtcNow;

            if (timeStatus.IsActive(utcNow))
            {
                var remainingTime = timeStatus.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                var content = $"{Context.User.Mention} następne drobne możesz otrzymać dopiero za {remainingTimeFriendly}!"
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            var mission = botuser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.WDaily);
            
            if (mission == null)
            {
                mission = new TimeStatus(StatusType.WDaily);
                botuser.TimeStatuses.Add(mission);
            }


            timeStatus.EndsAt = _systemClock.UtcNow.AddHours(20);
            botuser.ScCount += 100;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}" });

            await ReplyAsync("", embed: $"{Context.User.Mention} łap drobne na waciki!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("chce muta", RunMode = RunMode.Async)]
        [Alias("mute me", "chce mute")]
        [Summary("odbierasz darmowego muta od bota - na serio i nawet nie proś o odmutowanie")]
        [Remarks("")]
        public async Task GiveMuteAsync()
        {
            var user = Context.User as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config == null)
            {
                await ReplyAsync("", embed: "Serwer nie jest poprawnie skonfigurowany.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var notifChannel = Context.Guild.GetTextChannel(config.NotificationChannelId);
            var userRole = Context.Guild.GetRole(config.UserRoleId);
            var muteRole = Context.Guild.GetRole(config.MuteRoleId);

            if (muteRole == null)
            {
                await ReplyAsync("", embed: "Rola wyciszająca nie jest ustawiona.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            if (user.Roles.Contains(muteRole))
            {
                await ReplyAsync("", embed: $"{user.Mention} już jest wyciszony.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            const int daysInYear = 365;
            var duration = TimeSpan.FromDays(_randomNumberGenerator.GetRandomValue(daysInYear) + 1);

            var acceptPayload = new AcceptSession.AcceptSessionPayload
            {
                Bot = Context.Client.CurrentUser,
                NotifChannel = notifChannel,
                MuteRole = muteRole,
                UserRole = userRole,
                User = user,
                Duration = duration,
            };

            var session = new AcceptSession(user.Id, _systemClock.UtcNow, acceptPayload);
            _sessionManager.Remove(session);

            var content = $"{user.Mention} na pewno chcesz muta?".ToEmbedMessage(EMType.Error).Build();
            var msg = await ReplyAsync("", embed: content);
            await msg.AddReactionsAsync(new IEmote[] {
                Emojis.Checked,
                Emojis.DeclineEmote
            });

            acceptPayload.Message = msg;

            _sessionManager.Add(session);
        }

        [Command("zaskórniaki")]
        [Alias("hourly", "zaskorniaki")]
        [Summary("upadłeś tak nisko, że prosisz o SC pod marketem")]
        [Remarks(""), RequireCommandChannel]
        public async Task GiveHourlyScAsync()
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var hourly = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.Hourly);
            
            if (hourly == null)
            {
                hourly = new TimeStatus(StatusType.Hourly);
                botuser.TimeStatuses.Add(hourly);
            }

            var utcNow = _systemClock.UtcNow;

            if (hourly.IsActive(utcNow))
            {
                var remainingTime = hourly.RemainingTime(utcNow);
                var remainingTimeFriendly = remainingTime.Humanize(4);
                await ReplyAsync("", embed: $"{Context.User.Mention} następne zaskórniaki możesz otrzymać dopiero za {remainingTime}"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            hourly.EndsAt = _systemClock.UtcNow.AddHours(1);
            botuser.ScCount += 5;

            var mission = botuser.TimeStatuses.FirstOrDefault(x => x.Type == StatusType.DHourly);

            if (mission == null)
            {
                mission = new TimeStatus(StatusType.DHourly);
                botuser.TimeStatuses.Add(mission);
            }
            mission.Count(_systemClock.UtcNow);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{Context.User.Mention} łap piątaka!".ToEmbedMessage(EMType.Success).Build());
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
                await ReplyAsync("", embed: "Podano zbyt mało opcji do wylosowania.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var allOptions = _randomNumberGenerator.Shuffle(options).ToList();

            var delay = _randomNumberGenerator.GetRandomValue(100, 500);
            await Task.Delay(delay);

            var content = $"{Emotes.PinkArrow} {_randomNumberGenerator.GetOneRandomFrom(allOptions)}".ToEmbedMessage(EMType.Success).WithAuthor(new EmbedAuthorBuilder().WithUser(Context.User)).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("rzut")]
        [Alias("beat", "toss")]
        [Summary("bot wykonuje rzut monetą, wygrywasz kwotę, o którą się założysz")]
        [Remarks("reszka 10"), RequireCommandChannel]
        public async Task TossCoinAsync(
            [Summary("strona monety (orzeł/reszka)")]CoinSide side,
            [Summary("ilość SC")]int amount)
        {
            if (amount <= 0)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} na minusie?!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (botuser.ScCount < amount)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botuser.ScCount -= amount;
            var thrown = (CoinSide)_randomNumberGenerator.GetRandomValue(2);
            var embed = $"{Context.User.Mention} pudło! Obecnie posiadasz {botuser.ScCount} SC.".ToEmbedMessage(EMType.Error);

            botuser.Stats.Tail += (thrown == CoinSide.Tail) ? 1 : 0;
            botuser.Stats.Head += (thrown == CoinSide.Head) ? 1 : 0;

            if (thrown == side)
            {
                ++botuser.Stats.Hit;
                botuser.ScCount += amount * 2;
                botuser.Stats.IncomeInSc += amount;
                embed = $"{Context.User.Mention} trafiony zatopiony! Obecnie posiadasz {botuser.ScCount} SC.".ToEmbedMessage(EMType.Success);
            }
            else
            {
                ++botuser.Stats.Misd;
                botuser.Stats.ScLost += amount;
                botuser.Stats.IncomeInSc -= amount;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: embed.Build());
            await Context.Channel.SendFileAsync($"./Pictures/coin{(int)thrown}.png");
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

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (!botuser.ApplySlotMachineSetting(setting, value))
            {
                await ReplyAsync("", embed: $"Podano niewłaściwą wartość parametru!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{Context.User.Mention} zmienił nastawy automatu.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("automat")]
        [Alias("slot", "slot machine")]
        [Summary("grasz na jednorękim bandycie")]
        [Remarks("info"), RequireCommandChannel]
        public async Task PlayOnSlotMachineAsync(
            [Summary("typ (info - wyświetla informacje)")]string type = "game")
        {
            var info = string.Format(Strings.GameInfo, PsyduckEmoji);

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
            var botUser = await _userRepository.GetUserOrCreateAsync(discordUser.Id);

            var toPay = _slotMachine.ToPay(botUser);

            if (botUser.ScCount < toPay)
            {
                var content1 = $"{Context.User.Mention} brakuje Ci SC, aby za tyle zagrać.".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: content1);
                return;
            }

            //new SlotWickedRandom()
            var win = _slotMachine.Play(botUser);
            // var win = machine.Play(new SlotEqualRandom());
            botUser.ScCount += win - toPay;

            await _userRepository.SaveChangesAsync();

            var smConfig = botUser.SMConfig;

            _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });

            var psay = smConfig.PsayMode > 0 ? $"{PsyduckEmoji} " : " ";
            var beatValue = smConfig.Beat.Value();
            var multiplierValue = smConfig.Multiplier.Value();

            var slotMachineResult = string.Format(
                Strings.SlotMachineResult,
                psay,
                discordUser.Mention,
                _slotMachine.Draw(botUser),
                beatValue,
                multiplierValue,
                win);

            var content = slotMachineResult.ToEmbedMessage(EMType.Bot).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("podarujsc")]
        [Alias("donatesc")]
        [Summary("dajesz datek innemu graczowi w postaci SC obarczony 40% podatkiem")]
        [Remarks("Karna 2000"), RequireCommandChannel]
        public async Task GiveUserScAsync([Summary("użytkownik")]SocketGuildUser user, [Summary("liczba SC (min. 1000)")]uint value)
        {
            if (value < 1000)
            {
                await ReplyAsync("", embed: "Nie można podarować mniej niż 1000 SC.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (user.Id == Context.User.Id)
            {
                await ReplyAsync("", embed: "Coś tutaj nie gra.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (await _userRepository.ExistsByDiscordIdAsync(user.Id))
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var targetUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var thisUser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (thisUser.ScCount < value)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie masz wystarczającej ilości SC.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            thisUser.ScCount -= value;

            var newScCnt = (value * 60) / 100;
            targetUser.ScCount += newScCnt;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{thisUser.Id}", "users", $"user-{targetUser.Id}" });

            await ReplyAsync("", embed: $"{Context.User.Mention} podarował {user.Mention} {newScCnt} SC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("zagadka", RunMode = RunMode.Async)]
        [Alias("riddle")]
        [Summary("wypisuje losową zagadkę i podaje odpowiedź po 15 sekundach")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowRiddleAsync()
        {
            var riddles = new List<Question>();
            riddles = await _questionRepository.GetCachedAllQuestionsAsync();

            riddles = _randomNumberGenerator.Shuffle(riddles).ToList();
            var riddle = riddles.FirstOrDefault();

            riddle.RandomizeAnswers(_randomNumberGenerator);
            var msg = await ReplyAsync(riddle.Get());
            await msg.AddReactionsAsync(riddle.GetEmotes());

            await Task.Delay(15000);

            int answers = 0;
            var react = await msg.GetReactionUsersAsync(riddle.GetRightEmote(), 100).FlattenAsync();
            foreach (var addR in riddle.GetEmotes())
            {
                var re = await msg.GetReactionUsersAsync(addR, 100).FlattenAsync();
                if (re.Any(x => x.Id == Context.User.Id)) answers++;
            }

            await msg.RemoveAllReactionsAsync();

            if (react.Any(x => x.Id == Context.User.Id) && answers < 2)
            {
                await ReplyAsync("", false, $"{Context.User.Mention} zgadłeś!".ToEmbedMessage(EMType.Success).Build());
            }
            else if (answers > 1)
            {
                await ReplyAsync("", false, $"{Context.User.Mention} wybrałeś więcej jak jedną odpowiedź!".ToEmbedMessage(EMType.Error).Build());
            }
            else await ReplyAsync("", false, $"{Context.User.Mention} pudło!".ToEmbedMessage(EMType.Error).Build());
        }
    }
}