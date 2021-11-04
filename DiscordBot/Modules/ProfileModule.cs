using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Models;
using Sanakan.Preconditions;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.Session;
using Sanakan.Services.Session.Models;
using Sanakan.TaskQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Profil"), RequireUserRole]
    public class ProfileModule : SanakanModuleBase
    {
        private readonly IProfileService _profileService;
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IGameDeckRepository _gameDeckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _systemClock;

        public ProfileModule(
            IProfileService prof,
            ISessionManager sessionManager,
            ICacheManager cacheManager,
            IGuildConfigRepository guildConfigRepository,
            IGameDeckRepository gameDeckRepository,
            IUserRepository userRepository,
            ISystemClock systemClock)
        {
            _profileService = prof;
            _sessionManager = sessionManager;
            _cacheManager = cacheManager;
            _guildConfigRepository = guildConfigRepository;
            _gameDeckRepository = gameDeckRepository;
            _userRepository = userRepository;
            _systemClock = systemClock;
        }

        [Command("portfel", RunMode = RunMode.Async)]
        [Alias("wallet")]
        [Summary("wyświetla portfel użytkownika")]
        [Remarks("")]
        public async Task ShowWalletAsync([Summary("użytkownik (opcjonalne)")]SocketUser? socketUser = null)
        {
            var user = socketUser ?? Context.User;

            if (user == null)
            {
                return;
            }

            var botuser = await _userRepository.GetCachedFullUserAsync(user.Id);

            if (botuser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }
            
            var content = ($"**Portfel** {user.Mention}:\n\n{botuser?.ScCount} **SC**\n{botuser?.TcCount} **TC**\n{botuser?.AcCount} **AC**\n\n"
                + $"**PW**:\n{botuser?.GameDeck?.CTCount} **CT**\n{botuser?.GameDeck?.PVPCoins} **PC**")
                .ToEmbedMessage(EMType.Info)
                .Build();

            await ReplyAsync("", embed: content);
        }

        [Command("subskrypcje", RunMode = RunMode.Async)]
        [Alias("sub")]
        [Summary("wyświetla daty zakończenia subskrypcji")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowSubsAsync()
        {
            var botuser = await _userRepository.GetCachedFullUserAsync(Context.User.Id);
            var timeStatuses = botuser.TimeStatuses.Where(x => x.Type.IsSubType());

            string subs = "brak";

            if (timeStatuses.Any())
            {
                subs = "";
                var utcNow = _systemClock.UtcNow;
                foreach (var timeStatus in timeStatuses)
                {
                    subs += $"{timeStatus.ToView(utcNow)}\n";
                }
            }

            var content = $"**Subskrypcje** {Context.User.Mention}:\n\n{subs.ElipseTrimToLength(1950)}".ToEmbedMessage(EMType.Info).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("przyznaj role", RunMode = RunMode.Async)]
        [Alias("add role")]
        [Summary("dodaje samo zarządzaną role")]
        [Remarks("newsy"), RequireCommandChannel]
        public async Task AddRoleAsync([Summary("nazwa roli z wypisz role")]string name)
        {
            var user = Context.User as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var selfRole = config.SelfRoles.FirstOrDefault(x => x.Name == name);
            var gRole = Context.Guild.GetRole(selfRole?.Role ?? 0);

            if (gRole == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono roli `{name}`".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!user.Roles.Contains(gRole))
            {
                await user.AddRoleAsync(gRole);
            }

            var content = $"{user.Mention} przyznano role: `{name}`".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("zdejmij role", RunMode = RunMode.Async)]
        [Alias("remove role")]
        [Summary("zdejmuje samo zarządzaną role")]
        [Remarks("newsy"), RequireCommandChannel]
        public async Task RemoveRoleAsync([Summary("nazwa roli z wypisz role")]string name)
        {
            var user = Context.User as SocketGuildUser;

            if (user == null)
            {
                return;
            }

            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var selfRole = config.SelfRoles.FirstOrDefault(x => x.Name == name);
            var gRole = Context.Guild.GetRole(selfRole?.Role ?? 0);

            if (gRole == null)
            {
                await ReplyAsync("", embed: $"Nie odnaleziono roli `{name}`".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (user.Roles.Contains(gRole))
            {
                await user.RemoveRoleAsync(gRole);
            }

            var content = $"{user.Mention} zdjęto role: `{name}`".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("wypisz role", RunMode = RunMode.Async)]
        [Summary("wypisuje samo zarządzane role")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowRolesAsync()
        {
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

            if (config.SelfRoles.Count < 1)
            {
                await ReplyAsync("", embed: "Nie odnaleziono roli.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            string stringRole = "";
            foreach (var selfRole in config.SelfRoles)
            {
                var gRole = Context.Guild.GetRole(selfRole?.Role ?? 0);
                stringRole += $" `{selfRole.Name}` ";
            }

            await ReplyAsync($"**Dostępne role:**\n{stringRole}\n\nUżyj `s.przyznaj role [nazwa]` aby dodać lub `s.zdejmij role [nazwa]` odebrać sobie role.");
        }

        [Command("statystyki", RunMode = RunMode.Async)]
        [Alias("stats")]
        [Summary("wyświetla statystyki użytkownika")]
        [Remarks("karna")]
        public async Task ShowUserStatsAsync(
            [Summary("użytkownik (opcjonalne)")]SocketUser? socketUser = null)
        {
            var user = socketUser ?? Context.User;

            if (user == null)
            {
                return;
            }
            
            var databaseUser = await _userRepository.GetCachedFullUserAsync(user.Id);

            if (databaseUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var userStats = databaseUser.Stats;

            var parameters = new object[]
            {
                user.Mention,
                databaseUser.MessagesCount,
                databaseUser.CommandsCount,
                userStats.WastedTcOnCards,
                userStats.WastedTcOnCookies,
                userStats.ScLost,
                userStats.IncomeInSc,
                userStats.SlotMachineGames,
                userStats.Tail = userStats.Head,
                userStats.Hit,
                userStats.Misd,
                userStats.OpenedBoosterPacksActivity,
                userStats.OpenedBoosterPacks
            };

            var summary = string.Format(Strings.ProfileUserStats, parameters);

            var embed = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Description = summary.ElipseTrimToLength(1950),
            };

            await ReplyAsync("", embed: embed.Build());
        }

        [Command("idp", RunMode = RunMode.Async)]
        [Alias("iledopoziomu", "howmuchtolevelup", "hmtlup")]
        [Summary("wyświetla ile pozostało punktów doświadczenia do następnego poziomu")]
        [Remarks("karna")]
        public async Task ShowHowMuchToLevelUpAsync(
            [Summary("użytkownik(opcjonalne)")]SocketUser? socketUser = null)
        {
            var user = socketUser ?? Context.User;
            
            if (user == null)
            {
                return;
            }

            var botuser = await _userRepository.GetByDiscordIdAsync(user.Id);

            if (botuser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var content = $"{user.Mention} potrzebuje **{botuser.GetRemainingExp(botuser.Level + 1)}** punktów doświadczenia do następnego poziomu."
                .ToEmbedMessage(EMType.Info).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("topka", RunMode = RunMode.Async)]
        [Alias("top")]
        [Summary("wyświetla topke użytkowników")]
        [Remarks(""), RequireAnyCommandChannel]
        public async Task ShowTopAsync(
            [Summary("rodzaj topki (poziom/sc/tc/pc/ac/posty(m/ms)/kart(a/y/ym)/karma(-))/pvp(s)")]TopType type = TopType.Level)
        {
            var payload = new ListSession<string>.ListSessionPayload
            {
                Bot = Context.Client.CurrentUser,
            };

            var discordUserId = Context.User.Id;
            var session = new ListSession<string>(discordUserId, _systemClock.UtcNow, payload, SessionExecuteCondition.ReactionAdded);
            _sessionManager.RemoveIfExists<ListSession<string>>(discordUserId);

            var building = await ReplyAsync("", embed: $"🔨 Trwa budowanie topki...".ToEmbedMessage(EMType.Bot).Build());
            var users = await _userRepository.GetCachedAllUsersAsync();
            var topUsers = _profileService.GetTopUsers(users, type, _systemClock.UtcNow);
            payload.ListItems = _profileService.BuildListView(topUsers, type, Context.Guild);

            payload.Embed = new EmbedBuilder
            {
                Color = EMType.Info.Color(),
                Title = $"Topka {type.Name()}"
            };

            await building.DeleteAsync();
            var msg = await ReplyAsync("", embed: session.BuildPage(0));
            await msg.AddReactionsAsync(new[] {
                Emojis.LeftwardsArrow,
                Emojis.RightwardsArrow,
            });

            payload.Message = msg;
            _sessionManager.Add(session);
        }

        [Command("widok waifu")]
        [Alias("waifu view")]
        [Summary("przełącza widoczność waifu na pasku bocznym profilu użytkownika")]
        [Remarks(""), RequireAnyCommandChannel]
        public async Task ToggleWaifuViewInProfileAsync()
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            botuser.ShowWaifuInProfile = !botuser.ShowWaifuInProfile;

            var result = botuser.ShowWaifuInProfile ? "załączony" : "wyłączony";

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });
            
            var content = $"Podgląd waifu w profilu {Context.User.Mention} został {result}."
                .ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync("", embed: content);
        }

        [Command("profil", RunMode = RunMode.Async)]
        [Alias("profile")]
        [Summary("wyświetla profil użytkownika")]
        [Remarks("karna")]
        public async Task ShowUserProfileAsync(
            [Summary("użytkownik (opcjonalne)")]SocketGuildUser? socketGuildUser = null)
        {
            var user = socketGuildUser ?? Context.User as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            var allUsers = await _userRepository.GetCachedAllUsersLiteAsync();
            var botUser = allUsers.FirstOrDefault(x => x.Id == user.Id);

            if (botUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            botUser.GameDeck = await _gameDeckRepository.GetCachedUserGameDeckAsync(user.Id);
            var topPosition = allUsers
                .OrderByDescending(x => x.ExperienceCount)
                .ToList()
                .IndexOf(botUser) + 1;
            using var stream = await _profileService
                .GetProfileImageAsync(user, botUser, topPosition);

            await Context.Channel.SendFileAsync(stream, $"{user.Id}.png");
        }

        [Command("misje")]
        [Alias("quest")]
        [Summary("wyświetla postęp misji użytkownika")]
        [Remarks("tak"), RequireAnyCommandChannel]
        public async Task ShowUserQuestsProgressAsync(
            [Summary("czy odebrać nagrody?")]bool claim = false)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            var weeklyQuests = botuser.CreateOrGetAllWeeklyQuests();
            var dailyQuests = botuser.CreateOrGetAllDailyQuests();
            var utcNow = _systemClock.UtcNow;
            
            if (claim)
            {
                var rewards = new List<string>();
                var allClaimedBefore = dailyQuests.Count(x => x.IsClaimed(utcNow)) == dailyQuests.Count;
                foreach (var dailyQuest in dailyQuests)
                {
                    if (dailyQuest.CanClaim(utcNow))
                    {
                        dailyQuest.Claim(botuser);
                        rewards.Add(dailyQuest.Type.GetRewardString());
                    }
                }

                if (!allClaimedBefore && dailyQuests.Count(x => x.IsClaimed(utcNow)) == dailyQuests.Count)
                {
                    botuser.AcCount += 10;
                    rewards.Add("10 AC");
                }

                foreach (var weeklyQuest in weeklyQuests)
                {
                    if (weeklyQuest.CanClaim(utcNow))
                    {
                        weeklyQuest.Claim(botuser);
                        rewards.Add(weeklyQuest.Type.GetRewardString());
                    }
                }

                if (rewards.Count > 0)
                {
                    _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

                    await ReplyAsync("", embed: $"**Odebrane nagrody:**\n\n{string.Join("\n", rewards)}".ToEmbedMessage(EMType.Success).WithUser(Context.User).Build());
                    await _userRepository.SaveChangesAsync();
                    return;
                }

                await ReplyAsync("", embed: "Nie masz nic do odebrania.".ToEmbedMessage(EMType.Error).WithUser(Context.User).Build());
                return;
            }

            var parameters = new object[]
            {
                string.Join("\n", dailyQuests.Select(x => x.ToView(utcNow))),
                string.Join("\n", weeklyQuests.Select(x => x.ToView(utcNow)))
            };

            var content = string.Format(Strings.UserQuestsProgress, parameters)
                .ToEmbedMessage(EMType.Bot)
                .WithUser(Context.User).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("styl")]
        [Alias("style")]
        [Summary("zmienia styl profilu (koszt 3000 SC/1000 TC)")]
        [Remarks("1 https://i.imgur.com/8UK8eby.png"), RequireCommandChannel]
        public async Task ChangeStyleAsync(
            [Summary("typ stylu (statystyki(0), obrazek(1), brzydkie(2), karcianka(3))")]ProfileType type,
            [Summary("bezpośredni adres do obrazka gdy wybrany styl 1 lub 2 (325 x 272)")]string? imgUrl = null,
            [Summary("waluta (SC/TC)")]SCurrency currency = SCurrency.Sc)
        {
            var scCost = 3000;
            var tcCost = 1000;

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);

            if (botuser.ScCount < scCost && currency == SCurrency.Sc)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }
            if (botuser.TcCount < tcCost && currency == SCurrency.Tc)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby TC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            switch (type)
            {
                case ProfileType.Image:
                case ProfileType.StatisticsWithImage:
                    var res = await _profileService.SaveProfileImageAsync(imgUrl, $"{Paths.SavedData}/SR{botuser.Id}.png", 325, 272);
                    if (res == SaveResult.Success)
                    {
                        botuser.StatsReplacementProfileUri = $"{Paths.SavedData}/SR{botuser.Id}.png";
                        break;
                    }
                    else if (res == SaveResult.BadUrl)
                    {
                        await ReplyAsync("", embed: "Nie wykryto obrazka! Upewnij się, że podałeś poprawny adres!".ToEmbedMessage(EMType.Error).Build());
                        return;
                    }
                    await ReplyAsync("", embed: "Coś poszło nie tak, prawdopodobnie nie mam uprawnień do zapisu!".ToEmbedMessage(EMType.Error).Build());
                    return;

                default:
                    break;
            }

            if (currency == SCurrency.Sc)
            {
                botuser.ScCount -= scCost;
            }
            else
            {
                botuser.TcCount -= tcCost;
            }
            botuser.ProfileType = type;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            var content = $"Zmieniono styl profilu użytkownika: {Context.User.Mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("tło")]
        [Alias("tlo", "bg", "background")]
        [Summary("zmienia obrazek tła profilu (koszt 5000 SC/2500 TC)")]
        [Remarks("https://i.imgur.com/LjVxiv8.png"), RequireCommandChannel]
        public async Task ChangeBackgroundAsync(
            [Summary("bezpośredni adres do obrazka (450 x 145)")]string imgUrl,
            [Summary("waluta (SC/TC)")]SCurrency currency = SCurrency.Sc)
        {
            var tcCost = 2500;
            var scCost = 5000;

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            if (botuser.ScCount < scCost && currency == SCurrency.Sc)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby SC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }
            if (botuser.TcCount < tcCost && currency == SCurrency.Tc)
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie posiadasz wystarczającej liczby TC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var res = await _profileService.SaveProfileImageAsync(imgUrl, $"{Paths.SavedData}/BG{botuser.Id}.png", 450, 145, true);
            
            if (res == SaveResult.Success)
            {
                botuser.BackgroundProfileUri = $"{Paths.SavedData}/BG{botuser.Id}.png";
            }
            else if (res == SaveResult.BadUrl)
            {
                await ReplyAsync("", embed: "Nie wykryto obrazka! Upewnij się, że podałeś poprawny adres!".ToEmbedMessage(EMType.Error).Build());
                return;
            }
            else
            {
                await ReplyAsync("", embed: "Coś poszło nie tak, prawdopodobnie nie mam uprawnień do zapisu!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (currency == SCurrency.Sc)
            {
                botuser.ScCount -= scCost;
            }
            else
            {
                botuser.TcCount -= tcCost;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            var content = $"Zmieniono tło profilu użytkownika: {Context.User.Mention}!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("globalki")]
        [Alias("global")]
        [Summary("nadaje na miesiąc rangę od globalnych emotek (1000 TC)")]
        [Remarks(""), RequireCommandChannel]
        public async Task AddGlobalEmotesAsync()
        {
            var cost = 1000;
            var user = Context.User as SocketGuildUser;
            if (user == null)
            {
                return;
            }

            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            if (botuser.TcCount < cost)
            {
                await ReplyAsync("", embed: $"{user.Mention} nie posiadasz wystarczającej liczby TC!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
            var gRole = Context.Guild.GetRole(gConfig.GlobalEmotesRoleId);
            if (gRole == null)
            {
                await ReplyAsync("", embed: "Serwer nie ma ustawionej roli globalnych emotek.".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var guildid = Context.Guild.Id;

            var global = botuser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Globals 
                    && x.GuildId == guildid);

            if (global == null)
            {
                global = new TimeStatus(StatusType.Globals, guildid);
                botuser.TimeStatuses.Add(global);
            }

            if (!user.Roles.Contains(gRole))
            {
                await user.AddRoleAsync(gRole);
            }

            global.EndsAt = global.EndsAt.Value.AddMonths(1);
            botuser.TcCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} wykupił miesiąc globalnych emotek!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("kolor")]
        [Alias("color", "colour")]
        [Summary("zmienia kolor użytkownika (koszt TC/SC na liście)")]
        [Remarks("pink"), RequireCommandChannel]
        public async Task ToggleColorRoleAsync(
            [Summary("kolor z listy (none - lista)")]FColor color = FColor.None,
            [Summary("waluta (SC/TC)")]SCurrency currency = SCurrency.Tc)
        {
            var user = Context.User as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }

            if (color == FColor.None)
            {
                using var img = _profileService.GetColorList(currency);
                await Context.Channel.SendFileAsync(img, "list.png");
                return;
            }

            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var points = currency == SCurrency.Tc ? botuser.TcCount : botuser.ScCount;

            if (points < color.Price(currency))
            {
                await ReplyAsync("", embed: $"{user.Mention} nie posiadasz wystarczającej liczby {currency.ToString().ToUpper()}!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var guildId = Context.Guild.Id;

            var colort = botuser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Color
                    && x.GuildId == guildId);

            if (colort == null)
            {
                colort = new TimeStatus(StatusType.Color, guildId);
                botuser.TimeStatuses.Add(colort);
            }

            if (color == FColor.CleanColor)
            {
                colort.EndsAt = _systemClock.UtcNow;
                await _profileService.RomoveUserColorAsync(user);
            }
            else
            {
                if (_profileService.HasSameColor(user, color) && colort.IsActive(_systemClock.UtcNow))
                {
                    colort.EndsAt = colort.EndsAt.Value.AddMonths(1);
                }
                else
                {
                    await _profileService.RomoveUserColorAsync(user);
                    colort.EndsAt = _systemClock.UtcNow.AddMonths(1);
                }

                var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
                if (!await _profileService.SetUserColorAsync(user, gConfig.AdminRoleId, color))
                {
                    await ReplyAsync("", embed: $"Coś poszło nie tak!".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                if (currency == SCurrency.Tc)
                {
                    botuser.TcCount -= color.Price(currency);
                }
                else
                {
                    botuser.ScCount -= color.Price(currency);
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} wykupił kolor!".ToEmbedMessage(EMType.Success).Build());
        }
    }
}