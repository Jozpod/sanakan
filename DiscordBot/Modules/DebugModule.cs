using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Common.Extensions;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game;
using Sanakan.Game.Extensions;
using Sanakan.Game.Services;
using Sanakan.Preconditions;
using Sanakan.Services;
using Sanakan.Services.Commands;
using Sanakan.Services.PocketWaifu;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Utilities;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using Shinden;
using Shinden.API;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Sanakan.ShindenApi.Models;
using Discord.Rest;
using Sanakan.Common.Cache;

namespace Sanakan.DiscordBot.Modules
{
    [Name("Debug"), Group("dev"), DontAutoLoad, RequireDev]
    public class DebugModule : SanakanModuleBase
    {
        private readonly IWaifuService _waifuService;
        private readonly IWritableOptions<SanakanConfiguration> _config;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IHelperService _helperService;
        private readonly IShindenClient _shindenClient;
        private readonly IImageProcessor _imageProcessor;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;
        private readonly IResourceManager _resourceManager;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ITaskManager _taskManager;

        public DebugModule(
            IShindenClient shindenClient,
            IBlockingPriorityQueue blockingPriorityQueue,
            IWaifuService waifuService,
            IHelperService helperService,
            IImageProcessor imageProcessor,
            IWritableOptions<SanakanConfiguration> config,
            IUserRepository userRepository,
            ICardRepository cardRepository,
            IGuildConfigRepository guildConfigRepository,
            ISystemClock systemClock,
            ICacheManager cacheManager,
            IResourceManager resourceManager,
            IRandomNumberGenerator randomNumberGenerator,
            ITaskManager taskManager)
        {
            _shindenClient = shindenClient;
            _blockingPriorityQueue = blockingPriorityQueue;
            _helperService = helperService;
            _config = config;
            _waifuService = waifuService;
            _guildConfigRepository = guildConfigRepository;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
            _resourceManager = resourceManager;
            _randomNumberGenerator = randomNumberGenerator;
            _imageProcessor = imageProcessor;
            _taskManager = taskManager;
        }

        [Command("poke", RunMode = RunMode.Async)]
        [Summary("generuje obrazek safari")]
        [Remarks("1")]
        public async Task GeneratePokeImageAsync([Summary("nr grafiki")]int index)
        {
            try
            {
                var images = await _resourceManager.ReadFromJsonAsync<List<SafariImage>>(Paths.PokeList);

                if(images == null)
                {
                    await ReplyAsync("", embed: $"Nie znaleziono obrazkow safari".ToEmbedMessage(EMType.Error).Build());
                    return;
                }

                var character = (await _shindenClient.GetCharacterInfoAsync(2)).Value;
                var channel = (ITextChannel)Context.Channel;

                _ = await _waifuService.GetSafariViewAsync(
                    images[index],
                    _waifuService.GenerateNewCard(null, character), channel);
            }
            catch (Exception ex)
            {
                await ReplyAsync("", embed: $"Coś poszło nie tak: {ex.Message}".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("missingu", RunMode = RunMode.Async)]
        [Summary("generuje listę id użytkowników, których nie widzi bot na serwerach")]
        [Remarks("")]
        public async Task GenerateMissingUsersListAsync()
        {
            var guilds = await Context.Client.GetGuildsAsync();
            var allUserIds = new List<ulong>(1000);
            foreach (var guild in guilds)
            {
                var users = await guild.GetUsersAsync();
                allUserIds.AddRange(users.Select(pr => pr.Id));
            }

            var nonExistingIds = await _userRepository
                .GetByExcludedDiscordIdsAsync(allUserIds.Distinct());

            var content = string.Join("\n", nonExistingIds).ToEmbedMessage(EMType.Bot).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("blacklist")]
        [Summary("dodaje/usuwa użytkownika do czarnej listy")]
        [Remarks("Karna")]
        public async Task TransferCardAsync([Summary("użytkownik")]SocketGuildUser user)
        {
            var targetUser = await _userRepository.GetUserOrCreateAsync(user.Id);

            targetUser.IsBlacklisted = !targetUser.IsBlacklisted;
            await _userRepository.SaveChangesAsync();

            var discordUserId = Context.User.Id;
            var key = string.Format(CacheKeys.User, discordUserId);
            _cacheManager.ExpireTag(key, CacheKeys.Users);

            await ReplyAsync("", embed: $"{user.Mention} - blacklist: {targetUser.IsBlacklisted}".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("rmsg", RunMode = RunMode.Async)]
        [Summary("wysyła wiadomość na kanał w danym serwerze jako odpowiedź do podanej innej wiadomości")]
        [Remarks("15188451644 101155483 1231231 Nie masz racji!")]
        public async Task SendResponseMsgToChannelInGuildAsync(
            [Summary("id serwera")]ulong guildId,
            [Summary("id kanału")]ulong channelId,
            [Summary("id wiadomości")]ulong messageId,
            [Summary("treść wiadomości")][Remainder]string messageContent)
        {
            try
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                var channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                var message = await channel.GetMessageAsync(messageId);

                if (message is IUserMessage userMessage)
                {
                    await userMessage.ReplyAsync(messageContent);
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("smsg", RunMode = RunMode.Async)]
        [Summary("wysyła wiadomość na kanał w danym serwerze")]
        [Remarks("15188451644 101155483 elo ziomki")]
        public async Task SendMsgToChannelInGuildAsync(
            [Summary("id serwera")]ulong guildId,
            [Summary("id kanału")]ulong channelId,
            [Summary("treść wiadomości")][Remainder]string message)
        {
            try
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                var channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                await channel.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("semsg", RunMode = RunMode.Async)]
        [Summary("wysyła wiadomość w formie embed na kanał w danym serwerze")]
        [Remarks("15188451644 101155483 bot elo ziomki")]
        public async Task SendEmbedMsgToChannelInGuildAsync(
            [Summary("id serwera")]ulong guildId,
            [Summary("id kanału")]ulong channelId,
            [Summary("typ wiadomości(Neutral/Warning/Success/Error/Info/Bot)")]EMType type,
            [Summary("treść wiadomości")][Remainder]string content)
        {
            try
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                var channel = (IMessageChannel)await guild.GetChannelAsync(channelId);

                await channel.SendMessageAsync("", embed: content.ToEmbedMessage(type).Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("r2msg", RunMode = RunMode.Async)]
        [Summary("dodaje reakcje do wiadomości")]
        [Remarks("15188451644 101155483 825724399512453140 <:Redpill:455880209711759400>")]
        public async Task AddReactionToMsgOnChannelInGuildAsync(
            [Summary("id serwera")]ulong guildId,
            [Summary("id kanału")]ulong channelId,
            [Summary("id wiadomości")]ulong messageId,
            [Summary("reakcja")]string reaction)
        {
            try
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                var channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                var message = await channel.GetMessageAsync(messageId);

                if (Emote.TryParse(reaction, out var emote))
                {
                    await message.AddReactionAsync(emote);
                }
                else
                {
                    await message.AddReactionAsync(new Emoji(reaction));
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("cup")]
        [Summary("wymusza update na kartach")]
        [Remarks("3123 121")]
        public async Task ForceUpdateCardsAsync(
            [Summary("WID kart")]params ulong[] ids)
        {
            var cards = await _cardRepository.GetByIdsAsync(ids);

            if (!cards.Any())
            {
                await ReplyAsync("", embed: $"{Context.User.Mention} nie odnaleziono kart.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var card in cards)
            {
                try
                {
                    var result = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);
                    
                    if (result.Value == null)
                    {
                        card.Unique = true;
                        throw new Exception($"Couldn't get card info!");
                    }

                    var characterInfo = result.Value;
                    var pictureUrl = UrlHelpers.GetPersonPictureURL(characterInfo.PictureId.Value);
                    var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();

                    card.Unique = false;
                    card.Name = characterInfo.ToString();
                    card.ImageUrl = hasImage ? pictureUrl : null;
                    card.Title = characterInfo?.Relations?.OrderBy(x => x.CharacterId)
                        .FirstOrDefault()?
                        .Title ?? "????";

                    _waifuService.DeleteCardImageIfExist(card);
                }
                catch (Exception) { }
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);

            await ReplyAsync("", embed: $"Zaktualizowano {cards.Count} kart.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("rozdajm", RunMode = RunMode.Async)]
        [Summary("rozdaje karty kilka razy")]
        [Remarks("1 10 5 10")]
        public async Task GiveawayCardsMultiAsync(
            [Summary("id użytkownika")]ulong id,
            [Summary("liczba kart")]uint count,
            [Summary("czas w minutach")]uint duration = 5,
            [Summary("liczba powtórzeń")]uint repeat = 1)
        {
            for (uint i = 0; i < repeat; i++)
            {
                await GiveawayCardsAsync(id, count, duration);
                await _taskManager.Delay(TimeSpan.FromSeconds(10));
            }
        }

        [Command("rozdaj", RunMode = RunMode.Async)]
        [Summary("rozdaje karty")]
        [Remarks("1 10 5")]
        public async Task GiveawayCardsAsync(
            [Summary("id użytkownika")]ulong discordUserId,
            [Summary("liczba kart")]uint cardCount,
            [Summary("czas w minutach")]uint duration = 5)
        {
            var emote = Emotes.GreenChecked;
            var time = _systemClock.UtcNow.AddMinutes(duration);
            var guild = Context.Guild;

            var mention = "";
            IRole? mutedRole = null;
            var config = await _guildConfigRepository.GetCachedGuildFullConfigAsync(guild.Id);

            if (config != null)
            {
                var wRole = Context.Guild.GetRole(config.WaifuRoleId);
                if (wRole != null) mention = wRole.Mention;

                mutedRole = guild.GetRole(config.MuteRoleId);
            }

            var userMessage = await ReplyAsync(mention, embed: $"Loteria kart. Zareaguj {emote}, aby wziąć udział.\n\nKoniec `{time.ToShortTimeString()}:{time.Second.ToString("00")}`"
                .ToEmbedMessage(EMType.Bot).Build());
            await userMessage.AddReactionAsync(emote);

            await _taskManager.Delay(TimeSpan.FromMinutes(duration));
            await userMessage.RemoveReactionAsync(emote, Context.Client.CurrentUser);

            var reactions = await userMessage.GetReactionUsersAsync(emote, 300).FlattenAsync();
            var users = _randomNumberGenerator.Shuffle(reactions).ToList();

            IUser? winner = null;

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(1));
            var cancellationToken = cancellationTokenSource.Token;

            await Task.Run(async () =>
            {
                while (winner == null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!users.Any())
                    {
                        await userMessage.ModifyAsync(x => x.Embed = "Na loterie nie stawił się żaden użytkownik!"
                            .ToEmbedMessage(EMType.Error)
                            .Build());
                        return;
                    }

                    bool muted = false;
                    var selected = _randomNumberGenerator.GetOneRandomFrom(users);
                    if (mutedRole != null && selected is SocketGuildUser su)
                    {
                        if (su.Roles.Any(x => x.Id == mutedRole.Id))
                        {
                            muted = true;
                        }
                    }

                    var dUser = await _userRepository.GetCachedFullUserAsync(selected.Id);
                    if (dUser != null && !muted)
                    {
                        if (!dUser.IsBlacklisted)
                        {
                            winner = selected;
                        }
                    }
                    users.Remove(selected);
                }
            }, cancellationToken);

            _blockingPriorityQueue.TryEnqueue(new LotteryMessage
            {
                CardCount = cardCount,
                WinnerUserId = winner.Id,
                WinnerUser = winner,
                Channel = Context.Channel,
                InvokingUserId = Context.User.Id,
                DiscordUserId = discordUserId,
                UserMessage = userMessage,
            });

           
            await userMessage.RemoveAllReactionsAsync();
        }

        [Command("tranc"), Priority(1)]
        [Summary("przenosi kartę między użytkownikami")]
        [Remarks("User 41231 41232")]
        public async Task TransferUserCardAsync(
            [Summary("użytkownik")]SocketUser user,
            [Summary("WIDs")]params ulong[] wids) => await TransferCardAsync(user.Id, wids);

        [Command("tranc"), Priority(1)]
        [Summary("przenosi kartę między użytkownikami")]
        [Remarks("User 41231 41232")]
        public async Task TransferCardAsync(
            [Summary("id użytkownika")]ulong discordUserId,
            [Summary("WIDs")]params ulong[] wids)
        {
            if (!await _userRepository.ExistsByDiscordIdAsync(discordUserId))
            {
                await ReplyAsync("", embed: "W bazie nie ma użytkownika o podanym id!"
                    .ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var thisCards = await _cardRepository.GetByIdsAsync(wids, new CardQueryOptions {
                IncludeTagList = true,
            });

            if (thisCards.Count < 1)
            {
                await ReplyAsync("", embed: "Nie odnaleziono kart!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var reply = $"Karta {thisCards.First().GetString(false, false, true)} została przeniesiona.";
            
            if (thisCards.Count > 1)
            {
                reply = $"Przeniesiono {thisCards.Count} kart.";
            }

            foreach (var thisCard in thisCards)
            {
                thisCard.Active = false;
                thisCard.InCage = false;
                thisCard.TagList.Clear();
                thisCard.GameDeckId = discordUserId;
                thisCard.Expedition = ExpeditionCardType.None;
            }

            await _cardRepository.SaveChangesAsync();

            var discordUserkey = string.Format(CacheKeys.User, discordUserId);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users);

            await ReplyAsync("", embed: reply.ToEmbedMessage(EMType.Success).Build());
        }

        [Command("rmcards"), Priority(1)]
        [Summary("kasuje podane karty")]
        [Remarks("41231 41232")]
        public async Task RemoveCardsAsync([Summary("WIDs")]params ulong[] wids)
        {
            var thisCards = await _cardRepository.GetByIdsAsync(wids, new CardQueryOptions
            {
                IncludeTagList = true,
                IncludeArenaStats = true,
                AsSingleQuery = true,
            });

            if (thisCards.Count < 1)
            {
                await ReplyAsync("", embed: "Nie odnaleziono kart!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var reply = $"Karta {thisCards.First().GetString(false, false, true)} została skasowana.";
            
            if (thisCards.Count > 1)
            {
                reply = $"Skasowano {thisCards.Count} kart.";
            }

            foreach (var thisCard in thisCards)
            {
                _waifuService.DeleteCardImageIfExist(thisCard);
                _cardRepository.Remove(thisCard);
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);

            await ReplyAsync("", embed: reply.ToEmbedMessage(EMType.Success).Build());
        }

        [Command("level")]
        [Summary("ustawia podany poziom użytkownikowi")]
        [Remarks("Karna 1")]
        public async Task ChangeUserLevelAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("poziom")]ulong level)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(user.Id);

            bUser.Level = level;
            bUser.ExperienceCount = ExperienceUtils.CalculateExpForLevel(level);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{Context.User.Id}", "users" });

            var content = $"{user.Mention} ustawiono {level} poziom.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("mkick", RunMode = RunMode.Async)]
        [Summary("wyrzuca użytkowników z serwera")]
        [Remarks("Jupson Moe")]
        public async Task MultiKickAsync([Summary("użytkownicy")]params SocketGuildUser[] users)
        {
            var count = 0;

            foreach (var user in users)
            {
                try
                {
                    await user.KickAsync("Multi kick - łamanie regulaminu");
                    ++count;
                }
                catch (Exception) {}
            }

            await ReplyAsync("", embed: $"Wyrzucono {count} użytkowników.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("mban", RunMode = RunMode.Async)]
        [Summary("banuje użytkowników z serwera")]
        [Remarks("Jupson Moe")]
        public async Task MultiBankAsync([Summary("użytkownicy")]params SocketGuildUser[] users)
        {
            var count = 0;

            foreach (var user in users)
            {
                try
                {
                    await Context.Guild.AddBanAsync(user, 0, "Multi ban - łamanie regulaminu");
                    ++count;
                }
                catch (Exception) {}
            }

            await ReplyAsync("", embed: $"Zbanowano {count} użytkowników.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("restore"), Priority(1)]
        [Summary("przenosi kartę na nowo do użytkownika")]
        [Remarks("Sniku")]
        public async Task RestoreCardsAsync([Summary("użytkownik")]SocketGuildUser user)
        {
            var bUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var thisCards = await _cardRepository.GetByIdFirstOrLastOwnerAsync(user.Id);

            if (thisCards.Count < 1)
            {
                await ReplyAsync("", embed: "Nie odnaleziono kart!".ToEmbedMessage(EMType.Bot).Build());
                return;
            }

            var reply = $"Karta {thisCards.First().GetString(false, false, true)} została przeniesiona.";

            if (thisCards.Count > 1)
            {
                reply = $"Przeniesiono {thisCards.Count} kart.";
            }

            foreach (var thisCard in thisCards)
            {
                thisCard.Active = false;
                thisCard.InCage = false;
                thisCard.TagList.Clear();
                thisCard.GameDeckId = user.Id;
            }

            await _cardRepository.SaveChangesAsync();

            var discordUserkey = string.Format(CacheKeys.User, Context.User.Id);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users);

            await ReplyAsync("", embed: reply.ToEmbedMessage(EMType.Success).Build());
        }

        [Command("missingc", RunMode = RunMode.Async)]
        [Summary("generuje listę id kart, których właścicieli nie widzi bot na serwerach")]
        [Remarks("true")]
        public async Task GenerateMissingUsersCardListAsync(
            [Summary("czy wypisać id'ki")]bool ids = false)
        {
            var guilds = await Context.Client.GetGuildsAsync();
            var allUserIds = new List<ulong>(1000);
            
            foreach (var guild in guilds)
            {
                var users = await guild.GetUsersAsync();
                allUserIds.AddRange(users.Select(pr => pr.Id));
            }

            var nonExistingIds = await _cardRepository.GetByExcludedGameDeckIdsAsync(allUserIds.Distinct());
                
            await ReplyAsync("", embed: $"Kart: {nonExistingIds.Count}".ToEmbedMessage(EMType.Bot).Build());

            if (ids)
            {
                await ReplyAsync("", embed: string.Join("\n", nonExistingIds).ToEmbedMessage(EMType.Bot).Build());
            }
        }

        [Command("cstats", RunMode = RunMode.Async)]
        [Summary("generuje statystyki kart począwszy od podanej karty")]
        [Remarks("1")]
        public async Task GenerateCardStatsAsync([Summary("WID")]ulong wid)
        {
            var stringBuilder = new StringBuilder(100);
            var stats = new long[(int)Rarity.E + 1];
            var rarities = RarityExtensions.Rarities;

            foreach (var rarity in rarities)
            {
                var count = await _cardRepository.CountByRarityAndSucceedingIdAsync(rarity, wid);
                stringBuilder.AppendFormat("{0}: `{1}`\n", rarity, count);
            }

            var content = stringBuilder.ToString().ToEmbedMessage(EMType.Bot).Build();

            await ReplyAsync("", embed: content);
        }

        [Command("dusrcards"), Priority(1)]
        [Summary("usuwa karty użytkownika o podanym id z bazy")]
        [Remarks("845155646123")]
        public async Task RemoveCardsUserAsync([Summary("id użytkownika")]ulong id)
        {
            var user = await _userRepository.GetUserOrCreateAsync(id);
            user.GameDeck.Cards.Clear();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { "users", $"user-{id}" });
            var content = $"Karty użytkownika o id: `{id}` zostały skasowane.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync("", embed: content);
        }

        [Command("duser"), Priority(1)]
        [Summary("usuwa użytkownika o podanym id z bazy")]
        [Remarks("845155646123")]
        public async Task FactoryUserAsync(
            [Summary("id użytkownika")]ulong id,
            [Summary("czy usunąć karty?")]bool cards = false)
        {
            var fakeUser = await _userRepository.GetUserOrCreateAsync(1);
            var user = await _userRepository.GetUserOrCreateAsync(id);

            if (!cards)
            {
                foreach (var card in user.GameDeck.Cards)
                {
                    card.InCage = false;
                    card.TagList.Clear();
                    card.LastOwnerId = id;
                    card.GameDeckId = fakeUser.GameDeck.Id;
                }
            }

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { "users", $"user-{id}" });

            await ReplyAsync("", embed: $"Użytkownik o id: `{id}` został wymazany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tc duser"), Priority(1)]
        [Summary("usuwa dane użytkownika o podanym id z bazy i danej wartości tc")]
        [Remarks("845155646123 5000")]
        public async Task FactoryUserAsync(
            [Summary("id użytkownika")]ulong id,
            [Summary("wartość tc")]long value)
        {
            var user = await _userRepository.GetUserOrCreateAsync(id);
            var cards = user.GameDeck.Cards.OrderByDescending(x => x.CreatedOn).ToList();

            foreach (var card in cards)
            {
                value -= 50;
                user.GameDeck.Cards.Remove(card);

                if (value <= 0)
                    break;
            }

            if (value > 0)
            {
                var kct = value / 50;
                if (user.GameDeck.Karma > 0)
                {
                    user.GameDeck.Karma -= kct;
                    if (user.GameDeck.Karma < 0)
                        user.GameDeck.Karma = 0;
                }
                else
                {
                    user.GameDeck.Karma += kct;
                    if (user.GameDeck.Karma > 0)
                        user.GameDeck.Karma = 0;
                }

                user.GameDeck.CTCount -= kct;
                if (user.GameDeck.CTCount < 0)
                {
                    user.GameDeck.CTCount = 0;
                    kct = 0;
                }

                if (kct > 0)
                {
                    user.GameDeck.Items.Clear();
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { "users", $"user-{id}" });

            await ReplyAsync("", embed: $"Użytkownik o id: `{id}` został zrównany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("utitle"), Priority(1)]
        [Summary("aktualizuje tytuł karty")]
        [Remarks("ssało")]
        public async Task ChangeTitleCardAsync(
            [Summary("WID")]ulong wid,
            [Summary("tytuł")][Remainder]string? title = null)
        {
            var thisCard = await _cardRepository.GetByIdAsync(wid);

            if (thisCard == null)
            {
                await ReplyAsync("", embed: $"Taka karta nie istnieje.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (title != null)
            {
                thisCard.Title = title;
            }
            else
            {
                var result = await _shindenClient.GetCharacterInfoAsync(thisCard.CharacterId);
                if (result.Value != null)
                {
                    thisCard.Title = result.Value.Relations?
                        .OrderBy(x => x.CharacterId)?
                        .FirstOrDefault()?.Title ?? "????";
                }
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);

            await ReplyAsync("", embed: $"Nowy tytuł to: `{thisCard.Title}`".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("delq"), Priority(1)]
        [Summary("kasuje zagadkę o podanym id")]
        [Remarks("20}")]
        public async Task RemoveQuizAsync([Summary("id zagadki")]ulong id)
        {
            var question = await _questionRepository.GetByIdAsync(id);

            if (question == null)
            {
                await ReplyAsync("", embed: $"Zagadka o ID: `{id}` nie istnieje!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            _questionRepository.Remove(question);
            await _questionRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Quiz);
            await ReplyAsync("", embed: $"Zagadka o ID: `{id}` została skasowana!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("addq"), Priority(1)]
        [Summary("dodaje nową zagadkę")]
        [Remarks("{...}")]
        public async Task AddNewQuizAsync(
            [Summary("zagadka w formie jsona")][Remainder]string json)
        {
            try
            {
                var question = JsonSerializer.Deserialize<Question>(json);
                _questionRepository.Add(question);
                await _questionRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.Quiz);
                await ReplyAsync("", embed: $"Nowy zagadka dodana, jej ID to: `{question.Id}`".ToEmbedMessage(EMType.Success).Build());
            }
            catch (Exception)
            {
                await ReplyAsync("", embed: $"Coś poszło nie tak przy parsowaniu jsona!".ToEmbedMessage(EMType.Error).Build());
            }
        }

        [Command("chpp"), Priority(1)]
        [Summary("ustawia liczbę znaków na pakiet")]
        [Remarks("true")]
        public async Task SetCharCntPerPacketAsync(
            [Summary("liczba znaków")]long count,
            [Summary("true/false - czy zapisać")]bool save = false)
        {
            if (save) {
                await _config.UpdateAsync(opt =>
                {
                    opt.Experience.CharPerPacket = count;
                });
            }

            var content = $"Ustawiono próg `{count}` znaków na pakiet. `Zapisano: {save.GetYesNo()}`"
                .ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("chpe"), Priority(1)]
        [Summary("ustawia liczbę znaków na punkt doświadczenia")]
        [Remarks("true")]
        public async Task SetCharCntPerExpAsync(
            [Summary("liczba znaków")]long count,
            [Summary("true/false - czy zapisać")]bool save = false)
        {
            if (save)
            {
                await _config.UpdateAsync(opt =>
                {
                    opt.Experience.CharPerPoint = count;
                });
            }

            await ReplyAsync("", embed: $"Ustawiono próg `{count}` znaków na punkt doświadczenia. `Zapisano: {save.GetYesNo()}`"
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tsafari"), Priority(1)]
        [Summary("wyłącza/załącza safari")]
        [Remarks("true")]
        public async Task ToggleSafariAsync(
            [Summary("true/false - czy zapisać")]bool save = false)
        {
            if (save)
            {
                await _config.UpdateAsync(opt =>
                {
                    opt.Discord.SafariEnabled = !opt.Discord.SafariEnabled;
                });
            }

            await ReplyAsync("", embed: $"Safari: `{_config.Value.Discord.SafariEnabled.GetYesNo()}` `Zapisano: {save.GetYesNo()}`".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("twevent"), Priority(1)]
        [Summary("wyłącza/załącza event waifu")]
        [Remarks("")]
        public async Task ToggleWaifuEventAsync()
        {
            var state = _waifuService.EventState;
            _waifuService.EventState = !state;

            var content = $"Waifu event: `{(!state).GetYesNo()}`.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync("", embed: content);
        }

        [Command("wevent"), Priority(1)]
        [Summary("ustawia id eventu (kasowane są po restarcie)")]
        [Remarks("https://pastebin.com/raw/Y6a8gH5P")]
        public async Task SetWaifuEventIdsAsync(
            [Summary("link do pliku z id postaci oddzielnymi średnikami")]string url)
        {
            using var client = new HttpClient();

            var ids = new List<ulong>();
            var res = await client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
            {
                await ReplyAsync("", embed: $"Nie udało się odczytać pliku.".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            using var body = await res.Content.ReadAsStreamAsync();
            using var sr = new StreamReader(body);
            var content = await sr.ReadToEndAsync();

            try
            {
                ids = content.Split(";").Select(x => ulong.Parse(x)).ToList();
            }
            catch (Exception ex)
            {
                await ReplyAsync("", embed: $"Format pliku jest niepoprawny! ({ex.Message})".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (ids.Any())
            {
                _waifuService.SetEventIds(ids);
                await ReplyAsync("", embed: $"Ustawiono `{ids.Count}` id.".ToEmbedMessage(EMType.Success).Build());
                return;
            }
        }

        [Command("lvlbadge", RunMode = RunMode.Async)]
        [Summary("generuje przykładowy obrazek otrzymania poziomu")]
        [Remarks("")]
        public async Task GenerateLevelUpBadgeAsync(
            [Summary("użytkownik(opcjonalne)")]IGuildUser? socketGuildUser = null)
        {
            var user = socketGuildUser ?? Context.User as IGuildUser;

            if (user == null)
            {
                return;
            }

            var role = user.Guild.Roles
                .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                .OrderByDescending(pr => pr.Position)
                .First();

            var color = role.Color;

            using var badge = await _imageProcessor.GetLevelUpBadgeAsync(
                "Very very long nickname of trolly user",
                2154,
                user.GetUserOrDefaultAvatarUrl(),
                color);
            using var badgeStream = badge.ToPngStream();
            await Context.Channel.SendFileAsync(badgeStream, $"{user.Id}.png");
        }

        [Command("devr", RunMode = RunMode.Async)]
        [Summary("przyznaje lub odbiera role developera")]
        [Remarks("")]
        public async Task ToggleDeveloperRoleAsync()
        {
            var user = Context.User as SocketGuildUser;

            if (user == null)
            {
                return;
            }

            var devr = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Developer");
            
            if (devr == null)
            {
                return;
            }

            if (user.Roles.Contains(devr))
            {
                await user.RemoveRoleAsync(devr);
                await ReplyAsync("", embed: $"{user.Mention} stracił role deva."
                    .ToEmbedMessage(EMType.Success).Build());
            }
            else
            {
                await user.AddRoleAsync(devr);
                await ReplyAsync("", embed: $"{user.Mention} otrzymał role deva."
                    .ToEmbedMessage(EMType.Success).Build());
            }
        }

        [Command("gitem"), Priority(1)]
        [Summary("generuje przedmiot i daje go użytkownikowi")]
        [Remarks("User 2 1")]
        public async Task GenerateItemAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("przedmiot")]ItemType itemType,
            [Summary("liczba przedmiotów")]uint count = 1,
            [Summary("jakość przedmiotu")]Quality quality = Quality.Broken)
        {
            var item = itemType.ToItem(count, quality);
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);

            var thisItem = botuser
                .GameDeck
                .Items
                .FirstOrDefault(x => x.Type == item.Type 
                    && x.Quality == item.Quality);

            if (thisItem == null)
            {
                thisItem = item;
                botuser.GameDeck.Items.Add(thisItem);
            }
            else thisItem.Count += count;

            await _userRepository.SaveChangesAsync();

            var discordUserkey = string.Format(CacheKeys.User, botuser.Id);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users);

            var cnt = (count > 1) ? $" x{count}" : "";
            var content = $"{user.Mention} otrzymał _{item.Name}_{cnt}.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("gcard"), Priority(1)]
        [Summary("generuje kartę i daje ją użytkownikowi")]
        [Remarks("User 54861")]
        public async Task GenerateCardAsync(
            [Summary("użytkownik")]IGuildUser user,
            [Summary("id postaci na shinden (nie podanie - losowo)")]ulong? characterId = null,
            [Summary("jakość karty (nie podanie - losowo)")]Rarity rarity = Rarity.E)
        {
            CharacterInfo character;
            Card card;

            var test = Context.User as SocketGuildUser;

            if (characterId.HasValue) {
                character = (await _shindenClient.GetCharacterInfoAsync(characterId.Value)).Value;
            }
            else {
                character = await _waifuService.GetRandomCharacterAsync();
            }

            if (rarity == Rarity.E) {
                card = _waifuService.GenerateNewCard(user, character);
            } else {
                card = _waifuService.GenerateNewCard(user, character, rarity);
            }

            card.Source = CardSource.GodIntervention;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.GameDeck.Cards.Add(card);

            databaseUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);

            await _userRepository.SaveChangesAsync();

            var discordUserkey = string.Format(CacheKeys.User, databaseUser.Id);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users);

            var content = $"{user.Mention} otrzymał {card.GetString(false, false, true)}.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync("", embed: content);
        }

        [Command("ctou"), Priority(1)]
        [Summary("zamienia kartę na ultimate")]
        [Remarks("54861 Zeta 100 100 1000")]
        public async Task MakeUltimateCardAsync(
            [Summary("wid karty")]ulong id,
            [Summary("jakość karty")]Quality quality,
            [Summary("dodatkowy atak")]int atk = 0,
            [Summary("dodatkowa obrona")]int def = 0,
            [Summary("dodatkowe hp")]int hp = 0)
        {
            var card = await _cardRepository.GetByIdAsync(id, new CardQueryOptions
            {
                IncludeTagList = true,
                AsSingleQuery = true,
            });

            if (card == null)
            {
                await ReplyAsync("", embed: "W bazie nie ma takiej karty!".ToEmbedMessage(EMType.Error).Build());
                return;
            }

            card.PAS = PreAssembledFigure.None;
            card.Rarity = Rarity.SSS;
            card.FromFigure = true;
            card.Quality = quality;

            if (def != 0) card.DefenceBonus = def;
            if (atk != 0) card.AttackBonus = atk;
            if (hp != 0) card.HealthBonus = hp;

            await _cardRepository.SaveChangesAsync();

            _waifuService.DeleteCardImageIfExist(card);

            var discordUserkey = string.Format(CacheKeys.User, card.GameDeckId);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users);

            await ReplyAsync("", embed: $"Utworzono: {card.GetString(false, false, true)}.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("sc"), Priority(1)]
        [Summary("zmienia SC użytkownika o podaną wartość")]
        [Remarks("Sniku 10000")]
        public async Task ChangeUserScAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba SC")]long amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.ScCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            var content = $"{user.Mention} ma teraz {botuser.ScCount} SC".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("ac"), Priority(1)]
        [Summary("zmienia AC użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserAcAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba AC")]long amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.AcCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} ma teraz {botuser.AcCount} AC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tc"), Priority(1)]
        [Summary("zmienia TC użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserTcAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba TC")]long amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.TcCount += amount;

            await _userRepository.SaveChangesAsync();

            var discordUserkey = string.Format(CacheKeys.User, botuser.Id);
            _cacheManager.ExpireTag(discordUserkey, CacheKeys.Users);

            await ReplyAsync("", embed: $"{user.Mention} ma teraz {botuser.TcCount} TC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pc"), Priority(1)]
        [Summary("zmienia PC użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserPcAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba PC")]long amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.GameDeck.PVPCoins += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} ma teraz {botuser.GameDeck.PVPCoins} PC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ct"), Priority(1)]
        [Summary("zmienia CT użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserCtAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba CT")]long amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.GameDeck.CTCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} ma teraz {botuser.GameDeck.CTCount} CT".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("exp"), Priority(1)]
        [Summary("zmienia punkty doświadczenia użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserExpAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba punktów doświadczenia")]ulong amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.ExperienceCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            await ReplyAsync("", embed: $"{user.Mention} ma teraz {botuser.ExperienceCount} punktów doświadczenia.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ost"), Priority(1)]
        [Summary("zmienia liczbę punktów ostrzeżeń")]
        [Remarks("User 10000")]
        public async Task ChangeUserOstAsync(
            [Summary("użytkownik")]SocketGuildUser user,
            [Summary("liczba ostrzeżeń")]long amount)
        {
            var botuser = await _userRepository.GetUserOrCreateAsync(user.Id);
            botuser.WarningsCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(new string[] { $"user-{botuser.Id}", "users" });

            var content = $"{user.Mention} ma teraz {botuser.WarningsCount} punktów ostrzeżeń.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync("", embed: content);
        }

        [Command("sime"), Priority(1)]
        [Summary("symuluje wyprawę daną kartą")]
        [Remarks("12312 n")]
        public async Task SimulateExpeditionAsync(
            [Summary("WID")]ulong wid,
            [Summary("typ wyprawy")]ExpeditionCardType expedition = ExpeditionCardType.None,
            [Summary("czas w minutach")]int time = -1)
        {
            if (expedition == ExpeditionCardType.None)
            {
                return;
            }

            var botUser = await _userRepository.GetUserAndDontTrackAsync(Context.User.Id);
            var thisCard = botUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);
            if (thisCard == null)
            {
                return;
            }

            if (time > 0)
            {
                thisCard.ExpeditionDate = _systemClock.UtcNow.AddMinutes(-time);
            }

            thisCard.Expedition = expedition;
            var message = _waifuService.EndExpedition(botUser, thisCard, true);

            var content = $"Karta {thisCard.GetString(false, false, true)} wróciła z {expedition.GetName("ej")} wyprawy!\n\n{message}"
                .ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync("", embed: content);
        }

        [Command("kill", RunMode = RunMode.Async)]
        [Summary("wyłącza bota")]
        [Remarks("")]
        public async Task TurnOffAsync()
        {
            await ReplyAsync("", embed: "To dobry czas by umrzeć.".ToEmbedMessage(EMType.Bot).Build());
            
            var discordClient = (BaseDiscordClient)Context.Client;
            await discordClient.LogoutAsync();
            await _taskManager.Delay(TimeSpan.FromMilliseconds(1500));
            Environment.Exit(0);
        }

        [Command("update", RunMode = RunMode.Async)]
        [Summary("wyłącza bota z kodem 200")]
        [Remarks("")]
        public async Task TurnOffWithUpdateAsync()
        {
            await ReplyAsync("", embed: "To już czas?".ToEmbedMessage(EMType.Bot).Build());
            
            var discordClient = (BaseDiscordClient)Context.Client;
            await discordClient.LogoutAsync();

            System.IO.File.Create("./updateNow");
            await _taskManager.Delay(TimeSpan.FromMilliseconds(1500));
            Environment.Exit(200);
        }

        [Command("rmconfig", RunMode = RunMode.Async)]
        [Summary("wyświetla konfiguracje powiadomień na obecnym serwerze")]
        [Remarks("")]
        public async Task ShowRMConfigAsync()
        {
            var serverConfig = _config.Value
                .RMConfig
                .Where(x => x.GuildId == Context.Guild.Id 
                    || x.GuildId == 0)
                .ToList();

            if (serverConfig.Any())
            {
                await ReplyAsync("", embed: $"**RMC:**\n{string.Join("\n\n", serverConfig)}".ElipseTrimToLength(1900).ToEmbedMessage(EMType.Bot).Build());
                return;
            }
            await ReplyAsync("", embed: $"**RMC:**\n\nBrak.".ToEmbedMessage(EMType.Bot).Build());
        }

        [Command("mrmconfig")]
        [Summary("zmienia istniejący wpis w konfiguracji powiadomień w odniesieniu do serwera, lub tworzy nowy gdy go nie ma")]
        [Remarks("News 1232321314 1232412323 tak")]
        public async Task ChangeRMConfigAsync(
            [Summary("typ wpisu")]RichMessageType type,
            [Summary("id kanału")]ulong channelId,
            [Summary("id roli")]ulong roleId,
            [Summary("czy zapisać?")]bool save = false)
        {
            var guildId = Context.Guild.Id;
            var config = _config.Value;
            var thisRM = config.RMConfig
                .FirstOrDefault(x => x.Type == type
                    && x.GuildId == guildId);
            
            if (thisRM == null)
            {
                thisRM = new RichMessageConfig
                {
                    GuildId = guildId,
                    Type = type
                };
                config.RMConfig.Add(thisRM);
            }

            thisRM.ChannelId = channelId;
            thisRM.RoleId = roleId;

            if (save)
            {
                await _config.UpdateAsync(opt =>
                {
                    opt.RMConfig = config.RMConfig;
                });
            }

            await ReplyAsync("", embed: "Wpis został zmodyfikowany!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ignore"), Priority(1)]
        [Summary("dodaje serwer do ignorowanych lub usuwa go z listy")]
        [Remarks("News 1232321314 1232412323 tak")]
        public async Task IgnoreServerAsync()
        {
            var config = _config.Value;

            if (config.Discord.BlacklistedGuilds.Contains(Context.Guild.Id))
            {
                await _config.UpdateAsync(opt =>
                {
                    opt.Discord.BlacklistedGuilds.Remove(Context.Guild.Id);
                });
                await ReplyAsync("", embed: "Serwer został usunięty z czarnej listy.".ToEmbedMessage(EMType.Success).Build());
            }
            else
            {
                
                await _config.UpdateAsync(opt =>
                {
                    opt.Discord.BlacklistedGuilds.Add(Context.Guild.Id);
                });
                await ReplyAsync("", embed: "Serwer został dodany do czarnej listy.".ToEmbedMessage(EMType.Success).Build());
            }
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("wypisuje polecenia")]
        [Remarks("kasuj")]
        public async Task SendHelpAsync(
            [Summary("nazwa polecenia (opcjonalne)")][Remainder]string? command = null)
        {
            if (command == null)
            {
                await ReplyAsync(_helperService.GivePrivateHelp("Debug"));

                return;
            }

            try
            {
                var prefix = _config.Value.Discord.Prefix;
                if (Context.Guild != null)
                {
                    var gConfig = await _guildConfigRepository.GetCachedGuildFullConfigAsync(Context.Guild.Id);

                    if (gConfig?.Prefix != null)
                    {
                        prefix = gConfig.Prefix;
                    }
                }

                await ReplyAsync(_helperService.GiveHelpAboutPrivateCmd("Debug", command, prefix));
            }
            catch (Exception ex)
            {
                await ReplyAsync("", embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
            }
        }
    }
}
