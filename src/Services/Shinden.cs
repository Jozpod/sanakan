#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Sanakan.Extensions;
using Sanakan.Services.Session;
using Sanakan.Services.Session.Models;
using Shinden;
using Shinden.Models;

namespace Sanakan.Services
{
    public enum UrlParsingError
    {
        None, InvalidUrl, InvalidUrlForum
    }

    public class Shinden
    {
        private ShindenClient _shClient;
        private SessionManager _session;
        private ImageProcessing _img;

        public Shinden(ShindenClient client, SessionManager session, ImageProcessing img)
        {
            _shClient = client;
            _session = session;
            _img = img;
        }

        public UrlParsingError ParseUrlToShindenId(string url, out ulong shindenId)
        {
            shindenId = 0;
            var splited = url.Split('/');
            bool http = splited[0].Equals("https:") || splited[0].Equals("http:");
            int toChek = http ? 2 : 0;

            if (splited.Length < (toChek == 2 ? 5 : 3))
                return UrlParsingError.InvalidUrl;

            if (splited[toChek].Equals("shinden.pl") || splited[toChek].Equals("www.shinden.pl"))
            {
                if(splited[++toChek].Equals("user") || splited[toChek].Equals("animelist") || splited[toChek].Equals("mangalist"))
                {
                    var data = splited[++toChek].Split('-');
                    if (ulong.TryParse(data[0], out shindenId))
                        return UrlParsingError.None;
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
                temp += $"[{++i}] {item.ToString()}\n";
                if (temp.Length > 1800)
                {
                    toSend[messageNr] += "\n```";
                    toSend[++messageNr] += $"```ini\n[{i}] {item.ToString()}\n";
                    temp = "";
                }
                else toSend[messageNr] += $"[{i}] {item.ToString()}\n";
            }
            toSend[messageNr] += "```\nNapisz `koniec`, aby zamknąć menu.";

            return toSend;
        }

        public async Task SendSearchInfoAsync(SocketCommandContext context, string title, QuickSearchType type)
        {
            if (title.Equals("fate/loli")) title = "Fate/kaleid Liner Prisma Illya";

            var session = new SearchSession(context.User, _shClient);
            if (_session.SessionExist(session)) return;

            var res = await QuickSearchAsync(title, type);
            if (!res.Item1)
            {
                await context.Channel.SendMessageAsync("", false, GetResponseFromSearchCode(res.Item2).ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var list = res.Item3;
            var toSend = GetSearchResponse(list, "Wybierz tytuł, który chcesz wyświetlić poprzez wpisanie numeru odpowiadającemu mu na liście.");

            if (list.Count == 1)
            {
                var info = (await _shClient.Title.GetInfoAsync(list.First())).Body;
                await context.Channel.SendMessageAsync("", false, info.ToEmbed());
            }
            else
            {
                session.SList = list;
                await SendSearchResponseAsync(context, toSend, session);
            }
        }

        public async Task SendSearchResponseAsync(SocketCommandContext context, string[] toSend, SearchSession session)
        {
            var msg = new Discord.Rest.RestUserMessage[10];
            for (int index = 0; index < toSend.Length; index++)
                if (toSend[index] != null) msg[index] = await context.Channel.SendMessageAsync(toSend[index]);

            session.Messages = msg;
            await _session.TryAddSession(session);
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
            var userInfo = await GetUserInfoAsync(shindenId);
            if (userInfo is null) return null;

            var resLR = await GetUserLastReadedAsync(shindenId);
            var resLW = await GetUserLastWatchedAsync(shindenId);

            using (var image = await _img.GetSiteStatisticAsync(userInfo,
                user.Roles.OrderByDescending(x => x.Position).FirstOrDefault()?.Color ?? Discord.Color.DarkerGrey,
                resLR, resLW))
            {
                return image.ToPngStream();
            }
        }

        private async Task<List<ILastWatched>> GetUserLastWatchedAsync(ulong shindenId)
        {
            List<ILastWatched> lw = null;
            try
            {
                var response = await _shClient.User.GetLastWatchedAsync(shindenId);
                if (response.IsSuccessStatusCode()) lw = response.Body;
            }
            catch (Exception) {}
            return lw;
        }

        private async Task<List<ILastReaded>> GetUserLastReadedAsync(ulong shindenId)
        {
            List<ILastReaded> lr = null;
            try
            {
                var response = await _shClient.User.GetLastReadedAsync(shindenId);
                if (response.IsSuccessStatusCode()) lr = response.Body;
            }
            catch (Exception) {}
            return lr;
        }

        private async Task<IUserInfo> GetUserInfoAsync(ulong shindenId)
        {
            IUserInfo uInfo = null;
            try
            {
                var response = await _shClient.User.GetAsync(shindenId);
                if (response.IsSuccessStatusCode()) uInfo = response.Body;
            }
            catch (Exception) {}
            return uInfo;
        }

        private async Task<(bool, HttpStatusCode, List<IQuickSearch>)> QuickSearchAsync(string title, QuickSearchType type)
        {
            try
            {
                var res = await _shClient.Search.QuickSearchAsync(title, type);
                return (res.IsSuccessStatusCode(), res.Code, res.Body);
            }
            catch (Exception) {}
            return (false, HttpStatusCode.RequestTimeout, null);;
        }
    }
}