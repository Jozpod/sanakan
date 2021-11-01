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
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.TaskQueue;
using Sanakan.Game.Services;
using System.Net;
using System.Collections.Generic;
using System.IO;

namespace Sanakan.Modules
{
    [Name("Shinden"), RequireUserRole]
    public class ShindenModule : ModuleBase<SocketCommandContext>
    {
        public enum UrlParsingError
        {
            None,
            InvalidUrl,
            InvalidUrlForum
        }

        private readonly IShindenClient _shindenclient;
        private readonly ISessionManager _sessionManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserRepository _userRepository;
        private readonly ISystemClock _systemClock;
        private readonly IShindenClient _shindenClient;
        private readonly IImageProcessor _imageProcessor;

        public ShindenModule(
            IShindenClient client,
            ISessionManager session,

            ICacheManager cacheManager,
            IUserRepository userRepository,
            ISystemClock systemClock)
        {
            _shindenclient = client;
            _sessionManager = session;
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
            await SendSearchInfoAsync(Context, title, QuickSearchType.Anime);
        }

        [Command("manga", RunMode = RunMode.Async)]
        [Alias("komiks")]
        [Summary("wyświetla informacje o mandze")]
        [Remarks("Gintama")]
        public async Task SearchMangaAsync(
            [Summary("tytuł")][Remainder]string title)
        {
            await SendSearchInfoAsync(Context, title, QuickSearchType.Manga);
        }

        [Command("postać", RunMode = RunMode.Async)]
        [Alias("postac", "character")]
        [Summary("wyświetla informacje o postaci")]
        [Remarks("Gintoki")]
        public async Task SearchCharacterAsync(
            [Summary("imie")][Remainder]string name)
        {
            var discordUserId = Context.User.Id;
            var payload = new SearchSession.SearchSessionPayload();

            var session = new SearchSession(discordUserId, _systemClock.UtcNow, payload);
            
            if (_sessionManager.Exists<SearchSession>(discordUserId))
            {
                return;
            }

            var searchResult = await _shindenclient.SearchCharacterAsync(name);

            if (searchResult.Value == null)
            {
                var content = GetResponseFromSearchCode(System.Net.HttpStatusCode.BadRequest)
                    .ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync("", embed: content);
                return;
            }

            var list = searchResult.Value;
            var toSend = GetSearchResponse(list, "Wybierz postać, którą chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu jej na liście.");

            if (list.Count == 1)
            {
                var firstCharacter = list.First();
                var info = (await _shindenclient.GetCharacterInfoAsync(firstCharacter.Id)).Value;

                var embed = new EmbedBuilder()
                {
                    Title = $"{info} ({info.CharacterId})".ElipseTrimToLength(EmbedBuilder.MaxTitleLength),
                    Description = info?.Biography?.Biography?.ElipseTrimToLength(1000),
                    Color = EMType.Info.Color(),
                    ImageUrl = info.PictureUrl,
                    Fields = info.GetFields(),
                    Url = info.CharacterUrl,
                }.Build();

                await ReplyAsync("", false, embed);
            }
            else
            {
                payload.PList = list;
                await SendSearchResponseAsync(Context, toSend, session, payload);
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

            using var stream = await GetSiteStatisticAsync(botUser.ShindenId.Value, user);
                
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
            switch (ParseUrlToShindenId(url, out var shindenId))
            {
                case UrlParsingError.InvalidUrl:
                    await ReplyAsync("", embed: "Wygląda na to, że podałeś niepoprawny link.".ToEmbedMessage(EMType.Error).Build());
                    return;

                case UrlParsingError.InvalidUrlForum:
                    await ReplyAsync("", embed: "Wygląda na to, że podałeś link do forum zamiast strony.".ToEmbedMessage(EMType.Error).Build());
                    return;

                default:
                case UrlParsingError.None:
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

        public UrlParsingError ParseUrlToShindenId(string url, out ulong shindenId)
        {
            shindenId = 0;
            var splited = url.Split('/');
            bool http = splited[0].Equals("https:") || splited[0].Equals("http:");
            int toChek = http ? 2 : 0;

            if (splited.Length < (toChek == 2 ? 5 : 3))
            {
                return UrlParsingError.InvalidUrl;
            }

            if (splited[toChek].Equals("shinden.pl") || splited[toChek].Equals("www.shinden.pl"))
            {
                if (splited[++toChek].Equals("user") || splited[toChek].Equals("animelist") || splited[toChek].Equals("mangalist"))
                {
                    var data = splited[++toChek].Split('-');
                    if (ulong.TryParse(data[0], out shindenId))
                    {
                        return UrlParsingError.None;
                    }
                }
            }

            if (splited[toChek].Equals("forum.shinden.pl") || splited[toChek].Equals("www.forum.shinden.pl"))
                return UrlParsingError.InvalidUrlForum;

            return UrlParsingError.InvalidUrl;
        }

        public string[] GetSearchResponse(IEnumerable<object> list, string title)
        {
            string temp = "";
            int messageNr = 0;
            var toSend = new string[10];
            toSend[0] = $"{title}\n```ini\n";
            int i = 0;

            foreach (var item in list)
            {
                temp += $"[{++i}] {item}\n";
                if (temp.Length > 1800)
                {
                    toSend[messageNr] += "\n```";
                    toSend[++messageNr] += $"```ini\n[{i}] {item}\n";
                    temp = "";
                }
                else toSend[messageNr] += $"[{i}] {item}\n";
            }
            toSend[messageNr] += "```\nNapisz `koniec`, aby zamknąć menu.";

            return toSend;
        }

        public async Task SendSearchInfoAsync(SocketCommandContext context, string title, QuickSearchType type)
        {
            if (title.Equals("fate/loli")) {
                title = "Fate/kaleid Liner Prisma Illya";
            }

            var discordUser = context.User;

            var payload = new SearchSession.SearchSessionPayload();

            var session = new SearchSession(discordUser.Id, _systemClock.UtcNow, payload);

            if (_sessionManager.Exists<SearchSession>(discordUser.Id))
            {
                return;
            }

            var searchResult = await _shindenClient.QuickSearchAsync(title, type);

            if (searchResult.Value == null)
            {
                await context.Channel.SendMessageAsync("", false, GetResponseFromSearchCode(HttpStatusCode.BadRequest)
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var list = searchResult.Value;
            var toSend = GetSearchResponse(list, "Wybierz tytuł, który chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu mu na liście.");

            if (list.Count == 1)
            {
                var first = list.First();
                var info = (await _shindenClient.GetAnimeMangaInfoAsync(first.TitleId)).Value;
                await context.Channel.SendMessageAsync("", false, info.ToEmbed());
            }
            else
            {
                payload.SList = list;
                await SendSearchResponseAsync(context, toSend, session, payload);
            }
        }

        public async Task SendSearchResponseAsync(SocketCommandContext context, string[] toSend, SearchSession session, SearchSession.SearchSessionPayload payload)
        {
            var message = new Discord.Rest.RestUserMessage[10];
            for (var index = 0; index < toSend.Length; index++)
            {
                if (toSend[index] != null)
                {
                    message[index] = await context.Channel.SendMessageAsync(toSend[index]);
                }
            }

            payload.Messages = message;
            _sessionManager.Add(session);
        }

        public string GetResponseFromSearchCode(HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.NotFound:
                    return "Brak wyników!";

                default:
                    return $"Brak połączenia z Shindenem! ({code})";
            }
        }

        public async Task<Stream> GetSiteStatisticAsync(ulong shindenId, SocketGuildUser user)
        {
            var result = await _shindenClient.GetUserInfoAsync(shindenId);

            if (result.Value == null)
            {
                return null;
            }

            var shindenUser = result.Value;
            var resLR = await _shindenClient.GetLastReadedAsync(shindenId);
            var resLW = await _shindenClient.GetLastWatchedAsync(shindenId);
            var color = user.Roles.OrderByDescending(x => x.Position)
                .FirstOrDefault()?.Color ?? Discord.Color.DarkerGrey;

            using var image = await _imageProcessor.GetSiteStatisticAsync(
                shindenUser,
                color,
                null, //resLR.Value != null ? resLR.Value : null,
                null); //resLW.Value != null ? resLW.Value : null);

            return image.ToPngStream();
        }
    }
}