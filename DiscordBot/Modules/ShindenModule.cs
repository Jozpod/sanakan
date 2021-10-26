using Discord.Commands;
using Discord.WebSocket;
using Sanakan.Extensions;
using Sanakan.Preconditions;
using Sanakan.Services.Commands;
using Sanakan.Services.Session;
using Sanakan.Services.Session.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sanakan.Common;
using Sanakan.ShindenApi;
using Shinden;
using Sanakan.DAL.Repositories.Abstractions;
using Shinden.API;
using Discord;
using Sanakan.ShindenApi.Utilities;

namespace Sanakan.Modules
{
    [Name("Shinden"), RequireUserRole]
    public class ShindenModule : ModuleBase<SocketCommandContext>
    {
        private readonly IShindenClient _shindenclient;
        private readonly SessionManager _session;
        private readonly Services.Shinden _shindenService;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _systemClock;

        public ShindenModule(
            IShindenClient client,
            SessionManager session,
            Services.Shinden shinden,
            ICacheManager cacheManager,
            IUserRepository userRepository,
            ISystemClock systemClock)
        {
            _shindenclient = client;
            _session = session;
            _shindenService = shinden;
            _cacheManager = cacheManager;
            _userRepository = userRepository;
            _systemClock = systemClock;
        }

        [Command("odcinki", RunMode = RunMode.Async)]
        [Alias("episodes")]
        [Summary("wyświetla nowo dodane epizody")]
        [Remarks(""), RequireCommandChannel]
        public async Task ShowNewEpisodesAsync()
        {
            var response = await _shindenclient.GetNewEpisodesAsync();

            if (response.Value == null)
            {
                await ReplyAsync("", embed: "Nie udało się pobrać listy odcinków.".ToEmbedMessage(EMType.Error).Build());
                return;
            }
            
            var episodes = response.Value;
            var user = Context.User;

            if (episodes?.Count > 0)
            {
                var msg = await ReplyAsync("", embed: "Lista poszła na PW!".ToEmbedMessage(EMType.Success).Build());

                try
                {
                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    foreach (var episode in episodes)
                    {
                        var embed = episode.ToEmbed();
                        await dmChannel.SendMessageAsync("", false, embed);
                        await Task.Delay(500);
                    }

                    await dmChannel.CloseAsync();
                }
                catch (Exception ex)
                {
                    await msg.ModifyAsync(x => x.Embed = $"{user.Mention} nie udało się wyłać PW! ({ex.Message})".ToEmbedMessage(EMType.Error).Build());
                }

                return;
            }
        }

        [Command("anime", RunMode = RunMode.Async)]
        [Alias("bajka")]
        [Summary("wyświetla informacje o anime")]
        [Remarks("Soul Eater")]
        public async Task SearchAnimeAsync([Summary("tytuł")][Remainder]string title)
        {
            await _shindenService.SendSearchInfoAsync(Context, title, QuickSearchType.Anime);
        }

        [Command("manga", RunMode = RunMode.Async)]
        [Alias("komiks")]
        [Summary("wyświetla informacje o mandze")]
        [Remarks("Gintama")]
        public async Task SearchMangaAsync([Summary("tytuł")][Remainder]string title)
        {
            await _shindenService.SendSearchInfoAsync(Context, title, QuickSearchType.Manga);
        }

        [Command("postać", RunMode = RunMode.Async)]
        [Alias("postac", "character")]
        [Summary("wyświetla informacje o postaci")]
        [Remarks("Gintoki")]
        public async Task SearchCharacterAsync([Summary("imie")][Remainder]string name)
        {
            var session = new SearchSession(Context.User, _shindenclient);
            
            if (_session.SessionExist(session))
            {
                return;
            }

            var searchResult = await _shindenclient.SearchCharacterAsync(name);

            if (searchResult.Value == null)
            {
                var content = _shindenService.GetResponseFromSearchCode(System.Net.HttpStatusCode.BadRequest)
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            var list = searchResult.Value;
            var toSend = _shindenService.GetSearchResponse(list, "Wybierz postać, którą chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu jej na liście.");

            if (list.Count == 1)
            {
                var firstCharacter = list.First();
                var info = (await _shindenclient.GetCharacterInfoAsync(firstCharacter.Id)).Value;

                var embed = new EmbedBuilder()
                {
                    Title = $"{info} ({info.CharacterId})".TrimToLength(EmbedBuilder.MaxTitleLength),
                    Description = info?.Biography?.Biography?.TrimToLength(1000),
                    Color = EMType.Info.Color(),
                    ImageUrl = info.PictureUrl,
                    Fields = info.GetFields(),
                    Url = info.CharacterUrl,
                }.Build();

                await ReplyAsync("", false, embed);
            }
            else
            {
                session.PList = list;
                await _shindenService.SendSearchResponseAsync(Context, toSend, session);
            }
        }

        [Command("strona", RunMode = RunMode.Async)]
        [Alias("ile", "otaku", "site", "mangozjeb")]
        [Summary("wyświetla statystyki użytkownika z strony")]
        [Remarks("karna")]
        public async Task ShowSiteStatisticAsync(
            [Summary("użytkownik (opcjonalne)")]SocketGuildUser? socketGuildUser = null)
        {
            var user = socketGuildUser ?? Context.User as SocketGuildUser;
            
            if (user == null)
            {
                return;
            }
            
            var botUser = await _userRepository.GetCachedFullUserAsync(user.Id);

            if (botUser == null)
            {
                await ReplyAsync("", embed: "Ta osoba nie ma profilu bota.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (!botUser.ShindenId.HasValue)
            {
                await ReplyAsync("", embed: "Ta osoba nie połączyła konta bota z kontem na stronie.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            using var stream = await _shindenService.GetSiteStatisticAsync(botUser.ShindenId.Value, user);
                
            if (stream == null)
            {
                await ReplyAsync("", embed: $"Brak połączenia z Shindenem!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var profileUrl = UrlHelpers.GetProfileURL(botUser.ShindenId.Value);

            await Context.Channel.SendFileAsync(stream, $"{user.Id}.png", $"{profileUrl}");
        }

        [Command("połącz")]
        [Alias("connect", "polacz", "połacz", "polącz")]
        [Summary("łączy funkcje bota, z kontem na stronie")]
        [Remarks("https://shinden.pl/user/136-mo0nisi44")]
        public async Task ConnectAsync([Summary("adres do profilu")]string url)
        {
            switch (_shindenService.ParseUrlToShindenId(url, out var shindenId))
            {
                case Services.UrlParsingError.InvalidUrl:
                    await ReplyAsync("", embed: "Wygląda na to, że podałeś niepoprawny link.".ToEmbedMessage(EMType.Error).Build());
                    return;

                case Services.UrlParsingError.InvalidUrlForum:
                await ReplyAsync("", embed: "Wygląda na to, że podałeś link do forum zamiast strony.".ToEmbedMessage(EMType.Error).Build());
                    return;

                default:
                case Services.UrlParsingError.None:
                    break;
            }

            var userResult = await _shindenclient.GetUserInfoAsync(shindenId);

            if (userResult.Value == null)
            {
                await ReplyAsync("", embed: $"Wystapil blad podczas polaczenia konta".ToEmbedMessage(EMType.Error).Build());
                //await ReplyAsync("", embed: $"Brak połączenia z Shindenem! ({response.Code})".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var user = userResult.Value;
            var userNameInDiscord = (Context.User as SocketGuildUser).Nickname ?? Context.User.Username;

            if (!user.Name.Equals(userNameInDiscord))
            {
                await ReplyAsync("", embed: "Wykryto próbę podszycia się. Nieładnie!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (await _userRepository.ExistsByShindenIdAsync(shindenId))
            {
                await ReplyAsync("", embed: "Wygląda na to, że ktoś już połączył się z tym kontem.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var botuser = await _userRepository.GetUserOrCreateAsync(Context.User.Id);
            botuser.ShindenId = shindenId;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}" });

            await ReplyAsync("", embed: "Konta zostały połączone.".ToEmbedMessage(EMType.Success).Build());
            return;
            
        }
    }
}