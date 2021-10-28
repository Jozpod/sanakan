using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services;
using DiscordBot.Services.Session;
using Microsoft.EntityFrameworkCore;
using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.Session;
using Sanakan.Services.Session.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Modules
{
    [Name("Profil"), RequireUserRole]
    public class ProfileModule : ModuleBase<SocketCommandContext>
    {
        private readonly Services.ProfileService _profile;
        private readonly SessionManager _session;
        private readonly ICacheManager _cacheManager;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly IGameDeckRepository _gameDeckRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _systemClock;

        public ProfileModule(
            Services.ProfileService prof,
            SessionManager session,
            ICacheManager cacheManager,
            IGuildConfigRepository guildConfigRepository,
            IGameDeckRepository gameDeckRepository,
            IUserRepository userRepository,
            ISystemClock systemClock)
        {
            _profile = prof;
            _session = session;
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
            var rsubs = botuser.TimeStatuses.Where(x => x.Type.IsSubType());

            string subs = "brak";

            if (rsubs.Any())
            {
                subs = "";
                foreach (var sub in rsubs)
                {
                    subs += $"{sub.ToView()}\n";
                }
            }

            var content = $"**Subskrypcje** {Context.User.Mention}:\n\n{subs.TrimToLength(1950)}".ToEmbedMessage(EMType.Info).Build();
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
        public async Task ShowStatsAsync([Summary("użytkownik (opcjonalne)")]SocketUser? socketUser = null)
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

            await ReplyAsync("", embed: botuser.GetStatsView(user).Build());
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

            var content = $"{user.Mention} potrzebuje **{botuser.GetRemainingExp()}** punktów doświadczenia do następnego poziomu."
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
            var session = new ListSession<string>(Context.User, Context.Client.CurrentUser);
            await _session.KillSessionIfExistAsync(session);

            var building = await ReplyAsync("", embed: $"🔨 Trwa budowanie topki...".ToEmbedMessage(EMType.Bot).Build());
            var users = await _userRepository.GetCachedAllUsersAsync();
            session.ListItems = _profile.BuildListView(_profile.GetTopUsers(users, type), type, Context.Guild);

            session.Event = ExecuteOn.ReactionAdded;
            session.Embed = new EmbedBuilder
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

            session.Message = msg;
            await _session.TryAddSession(session);
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
            using var stream = await _profile
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

            if (claim)
            {
                var rewards = new List<string>();
                var allClaimedBefore = dailyQuests.Count(x => x.IsClaimed()) == dailyQuests.Count;
                foreach (var d in dailyQuests)
                {
                    if (d.CanClaim())
                    {
                        d.Claim(botuser);
                        rewards.Add(d.Type.GetRewardString());
                    }
                }

                if (!allClaimedBefore && dailyQuests.Count(x => x.IsClaimed()) == dailyQuests.Count)
                {
                    botuser.AcCount += 10;
                    rewards.Add("10 AC");
                }

                foreach (var w in weeklyQuests)
                {
                    if (w.CanClaim())
                    {
                        w.Claim(botuser);
                        rewards.Add(w.Type.GetRewardString());
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

            var dailyTip = "Za wykonanie wszystkich dziennych misji można otrzymać 10 AC.";
            var totalTip = "Dzienne misje odświeżają się o północy, a tygodniowe co niedzielę.";
            var daily = $"**Dzienne misje:**\n\n{string.Join("\n", dailyQuests.Select(x => x.ToView()))}";
            var weekly = $"**Tygodniowe misje:**\n\n{string.Join("\n", weeklyQuests.Select(x => x.ToView()))}";

            var content = $"{daily}\n\n{dailyTip}\n\n\n{weekly}\n\n{totalTip}".ToEmbedMessage(EMType.Bot).WithUser(Context.User).Build();
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
                case ProfileType.Img:
                case ProfileType.StatsWithImg:
                    var res = await _profile.SaveProfileImageAsync(imgUrl, $"{Paths.SavedData}/SR{botuser.Id}.png", 325, 272);
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
        public async Task ChangeBackgroundAsync([Summary("bezpośredni adres do obrazka (450 x 145)")]string imgUrl, [Summary("waluta (SC/TC)")]SCurrency currency = SCurrency.Sc)
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

            var res = await _profile.SaveProfileImageAsync(imgUrl, $"{Paths.SavedData}/BG{botuser.Id}.png", 450, 145, true);
            
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

            var global = botuser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Globals 
                    && x.Guild == Context.Guild.Id);

            if (global == null)
            {
                global = StatusType.Globals.NewTimeStatus(Context.Guild.Id);
                botuser.TimeStatuses.Add(global);
            }

            if (!user.Roles.Contains(gRole))
            {
                await user.AddRoleAsync(gRole);
            }

            global.EndsAt = global.EndsAt.AddMonths(1);
            botuser.TcCount -= cost;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} wykupił miesiąc globalnych emotek!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("kolor")]
        [Alias("color", "colour")]
        [Summary("zmienia kolor użytkownika (koszt TC/SC na liście)")]
        [Remarks("pink"), RequireCommandChannel]
        public async Task ToggleColorRoleAsync([Summary("kolor z listy (none - lista)")]FColor color = FColor.None, [Summary("waluta (SC/TC)")]SCurrency currency = SCurrency.Tc)
        {
            var user = Context.User as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }
            

            if (color == FColor.None)
            {
                using var img = _profile.GetColorList(currency);
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

            var colort = botuser.TimeStatuses
                .FirstOrDefault(x => x.Type == StatusType.Color
                    && x.Guild == Context.Guild.Id);

            if (colort == null)
            {
                colort = StatusType.Color.NewTimeStatus(Context.Guild.Id);
                botuser.TimeStatuses.Add(colort);
            }

            if (color == FColor.CleanColor)
            {
                colort.EndsAt = _systemClock.UtcNow;
                await _profile.RomoveUserColorAsync(user);
            }
            else
            {
                if (_profile.HasSameColor(user, color) && colort.IsActive())
                {
                    colort.EndsAt = colort.EndsAt.AddMonths(1);
                }
                else
                {
                    await _profile.RomoveUserColorAsync(user);
                    colort.EndsAt = _systemClock.UtcNow.AddMonths(1);
                }

                var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);
                if (!await _profile.SetUserColorAsync(user, gConfig.AdminRoleId, color))
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