using Discord;
using Discord.Commands;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.Common.Extensions;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Resources;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game;
using Sanakan.Game.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Preconditions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Utilities;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    [Name(PrivateModules.Debug), Group("dev"), DontAutoLoad, RequireDev]
    public class DebugModule : SanakanModuleBase
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IIconConfiguration _iconsConfiguration;
        private readonly IEventIdsImporter _eventIdsImporter;
        private readonly IFileSystem _fileSystem;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IShindenClient _shindenClient;
        private readonly IWaifuService _waifuService;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IHelperService _helperService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IWritableOptions<SanakanConfiguration> _config;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IGuildConfigRepository _guildConfigRepository;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;
        private readonly IResourceManager _resourceManager;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ITaskManager _taskManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceScope _serviceScope;

        public DebugModule(
            IApplicationLifetime applicationLifetime,
            IIconConfiguration iconsConfiguration,
            IEventIdsImporter eventIdsImporter,
            IFileSystem fileSystem,
            IDiscordClientAccessor discordClientAccessor,
            IShindenClient shindenClient,
            IBlockingPriorityQueue blockingPriorityQueue,
            IWaifuService waifuService,
            IHelperService helperService,
            IImageProcessor imageProcessor,
            IWritableOptions<SanakanConfiguration> config,
            ISystemClock systemClock,
            ICacheManager cacheManager,
            IResourceManager resourceManager,
            IRandomNumberGenerator randomNumberGenerator,
            ITaskManager taskManager,
            IServiceScopeFactory serviceScopeFactory)
        {
            _applicationLifetime = applicationLifetime;
            _iconsConfiguration = iconsConfiguration;
            _eventIdsImporter = eventIdsImporter;
            _fileSystem = fileSystem;
            _discordClientAccessor = discordClientAccessor;
            _shindenClient = shindenClient;
            _blockingPriorityQueue = blockingPriorityQueue;
            _helperService = helperService;
            _config = config;
            _waifuService = waifuService;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
            _resourceManager = resourceManager;
            _randomNumberGenerator = randomNumberGenerator;
            _imageProcessor = imageProcessor;
            _taskManager = taskManager;
            _serviceScopeFactory = serviceScopeFactory;

            _serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = _serviceScope.ServiceProvider;
            _guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _cardRepository = serviceProvider.GetRequiredService<ICardRepository>();
            _questionRepository = serviceProvider.GetRequiredService<IQuestionRepository>();
        }

        public override void Dispose()
        {
            _serviceScope.Dispose();
        }

        [Command("poke", RunMode = RunMode.Async)]
        [Summary("generuje obrazek safari")]
        [Remarks("1")]
        public async Task GeneratePokeImageAsync([Summary("nr grafiki")] int imageIndex)
        {
            Embed embed;

            try
            {
                var images = await _resourceManager.ReadFromJsonAsync<List<SafariImage>>(Paths.PokeList);

                if (images == null)
                {
                    embed = Strings.NoSafariImage.ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                var character = (await _shindenClient.GetCharacterInfoAsync(2)).Value!;
                var channel = (ITextChannel)Context.Channel;
                var card = _waifuService.GenerateNewCard(null, character);
                var image = images[imageIndex];

                _ = await _waifuService.GetSafariViewAsync(
                    image,
                    card,
                    channel);
            }
            catch (Exception ex)
            {
                var message = string.Format(Strings.ErrorOccurred, ex.Message);
                embed = message.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
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
            await ReplyAsync(embed: content);
        }

        [Command("blacklist")]
        [Summary("dodaje/usuwa użytkownika do czarnej listy")]
        [Remarks("Karna")]
        public async Task AddUserToBlackList(
            [Summary(ParameterInfo.User)] IUser user)
        {
            var targetUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            Embed embed;

            if (targetUser == null)
            {
                embed = Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            targetUser.IsBlacklisted = !targetUser.IsBlacklisted;
            await _userRepository.SaveChangesAsync();

            var discordUserId = Context.User.Id;
            _cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

            embed = $"{user.Mention} - blacklist: {targetUser.IsBlacklisted}".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("rmsg", RunMode = RunMode.Async)]
        [Summary("wysyła wiadomość na kanał w danym serwerze jako odpowiedź do podanej innej wiadomości")]
        [Remarks("15188451644 101155483 1231231 Nie masz racji!")]
        public async Task SendResponseMsgToChannelInGuildAsync(
            [Summary(ParameterInfo.GuildId)] ulong guildId,
            [Summary("id kanału")] ulong channelId,
            [Summary(ParameterInfo.MessageId)] ulong messageId,
            [Summary(ParameterInfo.MessageContent)][Remainder] string messageContent)
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
            [Summary(ParameterInfo.GuildId)] ulong guildId,
            [Summary("id kanału")] ulong channelId,
            [Summary("treść wiadomości")][Remainder] string message)
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
            [Summary(ParameterInfo.GuildId)] ulong guildId,
            [Summary("id kanału")] ulong channelId,
            [Summary("typ wiadomości(Neutral/Warning/Success/Error/Info/Bot)")] EMType type,
            [Summary("treść wiadomości")][Remainder] string content)
        {
            try
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                var channel = (IMessageChannel)await guild.GetChannelAsync(channelId);

                await channel.SendMessageAsync(embed: content.ToEmbedMessage(type).Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("r2msg", RunMode = RunMode.Async)]
        [Summary("dodaje reakcje do wiadomości")]
        [Remarks("15188451644 101155483 825724399512453140 <:Redpill:455880209711759400>")]
        public async Task AddReactionToMessageOnChannelInGuildAsync(
            [Summary(ParameterInfo.GuildId)] ulong guildId,
            [Summary("id kanału")] ulong channelId,
            [Summary(ParameterInfo.MessageId)] ulong messageId,
            [Summary("reakcja")] string reaction)
        {
            IEmote emote;

            try
            {
                var guild = await Context.Client.GetGuildAsync(guildId);
                var channel = (IMessageChannel)await guild.GetChannelAsync(channelId);
                var message = await channel.GetMessageAsync(messageId);

                if (Emote.TryParse(reaction, out var parsedEmote))
                {
                    emote = parsedEmote;
                }
                else
                {
                    emote = new Emoji(reaction);
                }

                await message.AddReactionAsync(emote);
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
            [Summary(ParameterInfo.WIDs)] params ulong[] ids)
        {
            var cards = await _cardRepository.GetByIdsAsync(ids);
            Embed embed;

            if (!cards.Any())
            {
                var message = string.Format(Strings.CardsNotFound, Context.User.Mention);
                await ReplyAsync(embed: message.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            foreach (var card in cards)
            {
                var result = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);

                if (result.Value == null)
                {
                    card.IsUnique = true;
                    await _cardRepository.SaveChangesAsync();
                    embed = $"Couldn't get card info!.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);
                    return;
                }

                var characterInfo = result.Value;
                var pictureUrl = UrlHelpers.GetPersonPictureURL(characterInfo.PictureId!.Value);
                var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();

                card.IsUnique = false;
                card.Name = characterInfo.ToString();
                card.ImageUrl = hasImage ? new Uri(pictureUrl) : null;
                card.Title = characterInfo?.Relations?.OrderBy(x => x.CharacterId)
                    .FirstOrDefault()?
                    .Title ?? Placeholders.Undefined;

                _waifuService.DeleteCardImageIfExist(card);
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);

            embed = $"Zaktualizowano {cards.Count} kart.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: embed);
        }

        [Command("rozdajm", RunMode = RunMode.Async)]
        [Summary("rozdaje karty kilka razy")]
        [Remarks("1 10 5 10")]
        public async Task GiveawayCardsMultiAsync(
            [Summary(ParameterInfo.User)] ulong id,
            [Summary("liczba kart")] uint count,
            [Summary(ParameterInfo.Duration)] TimeSpan duration,
            [Summary("liczba powtórzeń")] uint repeatCount = 1)
        {
            var delayTimespan = TimeSpan.FromSeconds(10);
#if DEBUG

            var totalLotteryTime = repeatCount * (delayTimespan + duration);
            var finishTime = _systemClock.UtcNow + totalLotteryTime;

            var embed = $"Koniec wszystkich loterii: {finishTime}"
                   .ToEmbedMessage(EMType.Info)
                   .Build();
            await ReplyAsync(embed: embed);
#endif

            for (uint i = 0; i < repeatCount; i++)
            {
                await GiveawayCardsAsync(id, count, duration);
                await _taskManager.Delay(delayTimespan);
            }
        }

        [Command("rozdaj", RunMode = RunMode.Async)]
        [Summary("rozdaje karty")]
        [Remarks("1 10 5")]
        public async Task GiveawayCardsAsync(
            [Summary(ParameterInfo.User)] ulong discordUserId,
            [Summary("liczba kart")] uint cardCount,
            [Summary(ParameterInfo.Duration)] TimeSpan duration)
        {
            var emote = _iconsConfiguration.GiveawayCardParticipate;
            var time = _systemClock.UtcNow + duration;
            var guild = Context.Guild;

            var mention = "";
            IRole? mutedRole = null;
            var config = await _guildConfigRepository.GetCachedById(guild.Id);
            Embed embed;

            if (config != null)
            {
                var wRole = guild.GetRole(config.WaifuRoleId!.Value);
                if (wRole != null)
                {
                    mention = wRole.Mention;
                }

                mutedRole = guild.GetRole(config.MuteRoleId);
            }

            embed = $"Loteria kart. Zareaguj {emote}, aby wziąć udział.\n\nKoniec `{time.ToShortTimeString()}:{time.Second:00}`"
                .ToEmbedMessage(EMType.Bot).Build();
            var userMessage = await ReplyAsync(mention, embed: embed);
            await userMessage.AddReactionAsync(emote);

            await _taskManager.Delay(duration);
            await userMessage.RemoveReactionAsync(emote, Context.Client.CurrentUser);

            var reactions = await userMessage.GetReactionUsersAsync(emote, 300).FlattenAsync();
            var users = _randomNumberGenerator.Shuffle(reactions).ToList();

            IUser? winner = null;

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(Durations.Minute);
            var cancellationToken = cancellationTokenSource.Token;

            try
            {
                await Task.Run(
                async () =>
                {
                    while (winner == null)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!users.Any())
                        {
                            embed = "Na loterie nie stawił się żaden użytkownik!"
                                .ToEmbedMessage(EMType.Error)
                                .Build();
                            await userMessage.ModifyAsync(x => x.Embed = embed);
                            return;
                        }

                        bool isMuted = false;
                        var selected = _randomNumberGenerator.GetOneRandomFrom(users);

                        if (mutedRole != null && selected is IGuildUser selectedUser)
                        {
                            isMuted = selectedUser.RoleIds.Any(id => id == mutedRole.Id);
                        }

                        var databaseUser = await _userRepository.GetCachedAsync(selected.Id);
                        if (databaseUser != null && !isMuted)
                        {
                            if (!databaseUser.IsBlacklisted)
                            {
                                winner = selected;
                            }
                        }

                        users.Remove(selected);
                    }
                },
                cancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }

            if (winner == null)
            {
                await userMessage.RemoveAllReactionsAsync();
                return;
            }

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
            [Summary(ParameterInfo.User)] IUser user,
            [Summary(ParameterInfo.WIDs)] params ulong[] wids) => await TransferCardAsync(user.Id, wids);

        [Command("tranc"), Priority(1)]
        [Summary("przenosi kartę między użytkownikami")]
        [Remarks("User 41231 41232")]
        public async Task TransferCardAsync(
            [Summary(ParameterInfo.User)] ulong discordUserId,
            [Summary(ParameterInfo.WIDs)] params ulong[] wids)
        {
            var mention = Context.User.Mention;
            Embed embed;

            if (!await _userRepository.ExistsByDiscordIdAsync(discordUserId))
            {
                embed = Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var cards = await _cardRepository.GetByIdsAsync(wids, new CardQueryOptions
            {
                IncludeTagList = true,
            });

            if (cards.Count < 1)
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var reply = $"Karta {cards.First().GetString(false, false, true)} została przeniesiona.";

            if (cards.Count > 1)
            {
                reply = $"Przeniesiono {cards.Count} kart.";
            }

            foreach (var card in cards)
            {
                card.Active = false;
                card.InCage = false;
                card.Tags.Clear();
                card.GameDeckId = discordUserId;
                card.Expedition = ExpeditionCardType.None;
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

            embed = reply.ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("rmcards"), Priority(1)]
        [Summary("kasuje podane karty")]
        [Remarks("41231 41232")]
        public async Task RemoveCardsAsync(
            [Summary(ParameterInfo.WIDs)] params ulong[] wids)
        {
            var cards = await _cardRepository.GetByIdsAsync(wids, new CardQueryOptions
            {
                IncludeTagList = true,
                IncludeArenaStats = true,
                AsSingleQuery = true,
            });
            var mention = Context.User.Mention;
            Embed embed;

            if (cards.Count < 1)
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var reply = $"Karta {cards.First().GetString(false, false, true)} została skasowana.";

            if (cards.Count > 1)
            {
                reply = $"Skasowano {cards.Count} kart.";
            }

            foreach (var card in cards)
            {
                _waifuService.DeleteCardImageIfExist(card);
                _cardRepository.Remove(card);
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);
            embed = reply.ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: embed);
        }

        [Command("level")]
        [Summary("ustawia podany poziom użytkownikowi")]
        [Remarks("Karna 1")]
        public async Task ChangeUserLevelAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("poziom")] ulong level)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);

            databaseUser.Level = level;
            databaseUser.ExperienceCount = ExperienceUtils.CalculateExpForLevel(level);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(Context.User.Id), CacheKeys.Users);

            var embed = $"{user.Mention} ustawiono {level} poziom.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("mkick", RunMode = RunMode.Async)]
        [Summary("wyrzuca użytkowników z serwera")]
        [Remarks("Jupson Moe")]
        public async Task MultiKickAsync(
            [Summary(ParameterInfo.Users)] params IGuildUser[] users)
        {
            var count = 0;

            foreach (var user in users)
            {
                try
                {
                    await user.KickAsync("Multi kick - łamanie regulaminu");
                    ++count;
                }
                catch (Exception)
                {
                }
            }

            await ReplyAsync(embed: $"Wyrzucono {count} użytkowników.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("mban", RunMode = RunMode.Async)]
        [Summary("banuje użytkowników z serwera")]
        [Remarks("Jupson Moe")]
        public async Task MultiBanAsync(
            [Summary(ParameterInfo.Users)] params IGuildUser[] users)
        {
            var count = 0;

            foreach (var user in users)
            {
                try
                {
                    await Context.Guild.AddBanAsync(user, 0, "Multi ban - łamanie regulaminu");
                    ++count;
                }
                catch (Exception)
                {
                }
            }

            await ReplyAsync(embed: $"Zbanowano {count} użytkowników.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("restore"), Priority(1)]
        [Summary("przenosi kartę na nowo do użytkownika")]
        [Remarks("Sniku")]
        public async Task RestoreCardsAsync(
            [Summary(ParameterInfo.User)] IGuildUser user)
        {
            var cards = await _cardRepository.GetByIdFirstOrLastOwnerAsync(user.Id);
            var mention = Context.User.Mention;
            Embed embed;

            if (cards.Count < 1)
            {
                embed = string.Format(Strings.CardsNotFound, mention).ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var reply = $"Karta {cards.First().GetString(false, false, true)} została przeniesiona.";

            if (cards.Count > 1)
            {
                reply = $"Przeniesiono {cards.Count} kart.";
            }

            foreach (var card in cards)
            {
                card.Active = false;
                card.InCage = false;
                card.Tags.Clear();
                card.GameDeckId = user.Id;
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(Context.User.Id), CacheKeys.Users);

            embed = reply.ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("missingc", RunMode = RunMode.Async)]
        [Summary("generuje listę id kart, których właścicieli nie widzi bot na serwerach")]
        [Remarks("true")]
        public async Task GenerateMissingUsersCardListAsync(
            [Summary("czy wypisać id'ki")] bool displayIds = false)
        {
            var guilds = await Context.Client.GetGuildsAsync();
            var allUserIds = new List<ulong>(1000);
            Embed embed;

            foreach (var guild in guilds)
            {
                var users = await guild.GetUsersAsync();
                allUserIds.AddRange(users.Select(pr => pr.Id));
            }

            var nonExistingIds = await _cardRepository.GetByExcludedGameDeckIdsAsync(allUserIds.Distinct());

            embed = $"Kart: {nonExistingIds.Count}".ToEmbedMessage(EMType.Bot).Build();
            await ReplyAsync(embed: embed);

            if (displayIds)
            {
                embed = string.Join("\n", nonExistingIds).ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("cstats", RunMode = RunMode.Async)]
        [Summary("generuje statystyki kart począwszy od podanej karty")]
        [Remarks("1")]
        public async Task GenerateCardStatsAsync([Summary(ParameterInfo.WID)] ulong wid)
        {
            var stringBuilder = new StringBuilder(100);
            var rarities = RarityExtensions.Rarities;

            foreach (var rarity in rarities)
            {
                var count = await _cardRepository.CountByRarityAndSucceedingIdAsync(rarity, wid);
                stringBuilder.AppendFormat("{0}: `{1}`\n", rarity, count);
            }

            var content = stringBuilder.ToString().ToEmbedMessage(EMType.Bot).Build();

            await ReplyAsync(embed: content);
        }

        [Command("dusrcards"), Priority(1)]
        [Summary("usuwa karty użytkownika o podanym id z bazy")]
        [Remarks("845155646123")]
        public async Task RemoveCardsUserAsync(
            [Summary(ParameterInfo.User)] ulong discordUserId)
        {
            var user = await _userRepository.GetUserOrCreateAsync(discordUserId);
            user.GameDeck.Cards.Clear();

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

            var content = $"Karty użytkownika o id: `{discordUserId}` zostały skasowane.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: content);
        }

        [Command("duser"), Priority(1)]
        [Summary("usuwa użytkownika o podanym id z bazy")]
        [Remarks("845155646123")]
        public async Task DeleteUserAsync(
            [Summary(ParameterInfo.User)] ulong discordUserId,
            [Summary("czy usunąć karty?")] bool deleteCards = false)
        {
            var fakeUser = await _userRepository.GetUserOrCreateAsync(DAL.Constants.RootUserId);
            var user = await _userRepository.GetUserOrCreateAsync(discordUserId);

            if (user == null)
            {
                await ReplyAsync(embed: Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            if (deleteCards)
            {
                foreach (var card in user.GameDeck.Cards)
                {
                    card.InCage = false;
                    card.Tags.Clear();
                    card.LastOwnerId = discordUserId;
                    card.GameDeckId = fakeUser.GameDeck.Id;
                }
            }

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

            await ReplyAsync(embed: $"Użytkownik o id: `{discordUserId}` został wymazany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tc duser"), Priority(1)]
        [Summary("usuwa dane użytkownika o podanym id z bazy i danej wartości tc")]
        [Remarks("845155646123 5000")]
        public async Task FactoryUserAsync(
            [Summary(ParameterInfo.User)] ulong discordUserId,
            [Summary("wartość tc")] long value)
        {
            var user = await _userRepository.GetUserOrCreateAsync(discordUserId);
            var gameDeck = user.GameDeck;
            var cards = gameDeck.Cards.OrderByDescending(x => x.CreatedOn).ToList();

            foreach (var card in cards)
            {
                value -= 50;
                gameDeck.Cards.Remove(card);

                if (value <= 0)
                {
                    break;
                }
            }

            if (value > 0)
            {
                var kct = value / 50;
                if (gameDeck.Karma > 0)
                {
                    gameDeck.Karma -= kct;
                    if (gameDeck.Karma < 0)
                    {
                        gameDeck.Karma = 0;
                    }
                }
                else
                {
                    user.GameDeck.Karma += kct;
                    if (gameDeck.Karma > 0)
                    {
                        gameDeck.Karma = 0;
                    }
                }

                gameDeck.CTCount -= kct;
                if (gameDeck.CTCount < 0)
                {
                    gameDeck.CTCount = 0;
                    kct = 0;
                }

                if (kct > 0)
                {
                    gameDeck.Items.Clear();
                }
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(discordUserId), CacheKeys.Users);

            await ReplyAsync(embed: $"Użytkownik o id: `{discordUserId}` został zrównany.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("utitle"), Priority(1)]
        [Summary("aktualizuje tytuł karty")]
        [Remarks("ssało")]
        public async Task ChangeTitleCardAsync(
            [Summary(ParameterInfo.WID)] ulong wid,
            [Summary("tytuł")][Remainder] string? title = null)
        {
            var card = await _cardRepository.GetByIdAsync(wid);
            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (title != null)
            {
                card.Title = title;
            }
            else
            {
                var result = await _shindenClient.GetCharacterInfoAsync(card.CharacterId);
                if (result.Value != null)
                {
                    card.Title = result.Value.Relations?
                        .OrderBy(x => x.CharacterId)?
                        .FirstOrDefault()?.Title ?? Placeholders.Undefined;
                }
            }

            await _cardRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Users);

            embed = $"Nowy tytuł to: `{card.Title}`".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("delq"), Priority(1)]
        [Summary("kasuje zagadkę o podanym id")]
        [Remarks("20}")]
        public async Task RemoveQuizAsync([Summary("id zagadki")] ulong id)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            Embed embed;

            if (question == null)
            {
                embed = "Zagadka o ID: `{id}` nie istnieje!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            _questionRepository.Remove(question);
            await _questionRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.Quiz(id));
            embed = $"Zagadka o ID: `{id}` została skasowana!".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("addq"), Priority(1)]
        [Summary("dodaje nową zagadkę")]
        [Remarks("{...}")]
        public async Task AddNewQuizAsync(
            [Summary("zagadka w formie jsona")][Remainder] string json)
        {
            Embed embed;

            try
            {
                var question = JsonSerializer.Deserialize<Question>(json);
                _questionRepository.Add(question!);
                await _questionRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(CacheKeys.Quizes);
                embed = $"Nowy zagadka dodana, jej ID to: `{question.Id}`".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }
            catch (Exception)
            {
                embed = $"Coś poszło nie tak przy parsowaniu jsona!".ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("chpp"), Priority(1)]
        [Summary("ustawia liczbę znaków na pakiet")]
        [Remarks("true")]
        public async Task SetCharCountPerPacketAsync(
            [Summary("liczba znaków")] ulong count,
            [Summary("true/false - czy zapisać")] bool save = false)
        {
            await _config.UpdateAsync(
            opt =>
            {
                opt.Experience.CharPerPacket = count;
            },
            save);

            var embed = $"Ustawiono próg `{count}` znaków na pakiet. `Zapisano: {save.GetYesNo()}`"
                .ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("chpe"), Priority(1)]
        [Summary("ustawia liczbę znaków na punkt doświadczenia")]
        [Remarks("true")]
        public async Task SetCharCountPerExpAsync(
            [Summary("liczba znaków")] long count,
            [Summary("true/false - czy zapisać")] bool save = false)
        {
            await _config.UpdateAsync(
            opt =>
            {
                opt.Experience.CharPerPoint = count;
            },
            save);

            await ReplyAsync(embed: $"Ustawiono próg `{count}` znaków na punkt doświadczenia. `Zapisano: {save.GetYesNo()}`"
                .ToEmbedMessage(EMType.Success).Build());
        }

        [Command("turlban"), Priority(1)]
        [Summary("wyłącza/załącza banowanie za spam url")]
        [Remarks("")]
        public async Task ToggleBanIfDisallowedUrlAsync()
        {
            var hasSaved = await _config.UpdateAsync(opt =>
            {
                opt.Discord.BanForUrlSpam = !opt.Discord.BanForUrlSpam;
            });

            Embed embed;

            if (hasSaved)
            {
                embed = $"Banowanie uzytkownikow za spamowanie url: `{_config.Value.Discord.BanForUrlSpam}`".ToEmbedMessage(EMType.Success).Build();
            }
            else
            {
                embed = $"Wystapil blad podczas zapisywania".ToEmbedMessage(EMType.Error).Build();
            }

            await ReplyAsync(embed: embed);
        }

        [Command("tsafari"), Priority(1)]
        [Summary("wyłącza/załącza safari")]
        [Remarks("true")]
        public async Task ToggleSafariAsync(
            [Summary("true/false - czy zapisać")] bool save = false)
        {
            await _config.UpdateAsync(
            opt =>
            {
                opt.Discord.SafariEnabled = !opt.Discord.SafariEnabled;
            },
            save);

            var embed = $"Safari: `{_config.Value.Discord.SafariEnabled.GetYesNo()}` `Zapisano: {save.GetYesNo()}`"
                .ToEmbedMessage(EMType.Success)
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("twevent"), Priority(1)]
        [Summary("wyłącza/załącza event waifu")]
        [Remarks("")]
        public async Task ToggleWaifuEventAsync()
        {
            var state = _waifuService.EventState;
            _waifuService.EventState = !state;

            var content = $"Waifu event: `{(!state).GetYesNo()}`.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: content);
        }

        [Command("wevent"), Priority(1)]
        [Summary("ustawia id eventu (kasowane są po restarcie)")]
        [Remarks("https://pastebin.com/raw/Y6a8gH5P")]
        public async Task SetWaifuEventIdsAsync(
            [Summary("link do pliku z id postaci oddzielnymi średnikami")] Uri url)
        {
            var result = await _eventIdsImporter.RunAsync(url);
            Embed embed;

            switch (result.State)
            {
                case EventIdsImporterState.Ok:
                    var ids = result.Value;
                    if (ids.Any())
                    {
                        _waifuService.SetEventIds(ids);
                        embed = $"Ustawiono `{ids.Count()}` id.".ToEmbedMessage(EMType.Success).Build();
                        await ReplyAsync(embed: embed);
                    }

                    break;
                case EventIdsImporterState.InvalidStatusCode:
                    embed = $"Nie udało się odczytać pliku.".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);

                    break;
                case EventIdsImporterState.InvalidFileFormat:
                    embed = $"Format pliku jest niepoprawny! ({result.Exception.Message})".ToEmbedMessage(EMType.Error).Build();
                    await ReplyAsync(embed: embed);

                    break;
            }
        }

        [Command("lvlbadge", RunMode = RunMode.Async)]
        [Summary("generuje przykładowy obrazek otrzymania poziomu")]
        [Remarks("")]
        public async Task GenerateLevelUpBadgeAsync(
            [Summary(ParameterInfo.UserOptional)] IGuildUser? guildUser = null)
        {
            var user = guildUser ?? Context.User as IGuildUser;

            if (user == null)
            {
                await ReplyAsync(embed: Strings.UserNotFound.ToEmbedMessage(EMType.Error).Build());
                return;
            }

            var role = user.Guild.Roles
                .Join(user.RoleIds, pr => pr.Id, pr => pr, (src, dst) => src)
                .OrderByDescending(pr => pr.Position)
                .First();

            var color = role.Color;
            var name = "Very very long nickname of trolly user";
            var level = 2154ul;

            using var badge = await _imageProcessor.GetLevelUpBadgeAsync(
                name,
                level,
                user.GetUserOrDefaultAvatarUrl(),
                color);
            using var badgeStream = badge.ToPngStream();
            await Context.Channel.SendFileAsync(badgeStream, $"{user.Id}.png");
        }

        [Command("devr", RunMode = RunMode.Async)]
        [Summary("przyznaje lub odbiera role developera")]
        [Remarks(""), RequireGuildUser]
        public async Task ToggleDeveloperRoleAsync()
        {
            var user = (IGuildUser)Context.User;
            Embed embed;

            var developerRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Developer");

            if (developerRole == null)
            {
                return;
            }

            if (user.RoleIds.Contains(developerRole.Id))
            {
                await user.RemoveRoleAsync(developerRole);
                embed = $"{user.Mention} stracił role deva.".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }
            else
            {
                await user.AddRoleAsync(developerRole);
                embed = $"{user.Mention} otrzymał role deva.".ToEmbedMessage(EMType.Success).Build();
                await ReplyAsync(embed: embed);
            }
        }

        [Command("gitem"), Priority(1)]
        [Summary("generuje przedmiot i daje go użytkownikowi")]
        [Remarks("User 2 1")]
        public async Task GenerateItemAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("przedmiot")] ItemType itemType,
            [Summary("liczba przedmiotów")] uint count = 1,
            [Summary("jakość przedmiotu")] Quality quality = Quality.Broken)
        {
            var item = itemType.ToItem(count, quality);
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;

            var gameDeckItem = gameDeck.Items
                .FirstOrDefault(x => x.Type == item.Type
                    && x.Quality == item.Quality);

            if (gameDeckItem == null)
            {
                gameDeckItem = item;
                gameDeck.Items.Add(gameDeckItem);
            }
            else
            {
                gameDeckItem.Count += count;
            }

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var countFormatted = (count > 1) ? $" x{count}" : "";
            var content = $"{user.Mention} otrzymał _{item.Name}_{countFormatted}.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("gcard"), Priority(1)]
        [Summary("generuje kartę i daje ją użytkownikowi")]
        [Remarks("User 54861")]
        public async Task GenerateCardAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("id postaci na shinden (nie podanie - losowo)")] ulong? characterId = null,
            [Summary("jakość karty (nie podanie - losowo)")] Rarity rarity = Rarity.E)
        {
            CharacterInfo? character;
            Card card;

            if (characterId.HasValue)
            {
                var characterInfoResult = await _shindenClient.GetCharacterInfoAsync(characterId.Value);

                if (characterInfoResult.Value == null)
                {
                    return;
                }

                character = characterInfoResult.Value;
            }
            else
            {
                character = await _waifuService.GetRandomCharacterAsync();

                if (character == null)
                {
                    return;
                }
            }

            if (rarity == Rarity.E)
            {
                card = _waifuService.GenerateNewCard(user.Id, character);
            }
            else
            {
                card = _waifuService.GenerateNewCard(user.Id, character, rarity);
            }

            card.Source = CardSource.GodIntervention;
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            var gameDeck = databaseUser.GameDeck;
            gameDeck.Cards.Add(card);

            gameDeck.RemoveCharacterFromWishList(card.CharacterId);

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var content = $"{user.Mention} otrzymał {card.GetString(false, false, true)}.".ToEmbedMessage(EMType.Success).Build();

            await ReplyAsync(embed: content);
        }

        [Command("ctou"), Priority(1)]
        [Summary("zamienia kartę na ultimate")]
        [Remarks("54861 Zeta 100 100 1000")]
        public async Task MakeUltimateCardAsync(
            [Summary(ParameterInfo.WID)] ulong cardId,
            [Summary("jakość karty")] Quality quality,
            [Summary("dodatkowy atak")] int? attackPoints = null,
            [Summary("dodatkowa obrona")] int? defencePoints = null,
            [Summary("dodatkowe hp")] int? healthPoints = null)
        {
            var card = await _cardRepository.GetByIdAsync(cardId, new CardQueryOptions
            {
                IncludeTagList = true,
                AsSingleQuery = true,
            });
            Embed embed;

            if (card == null)
            {
                embed = Strings.CardNotFound.ToEmbedMessage(EMType.Error).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            card.PAS = PreAssembledFigure.None;
            card.Rarity = Rarity.SSS;
            card.FromFigure = true;
            card.Quality = quality;

            if (defencePoints.HasValue)
            {
                card.DefenceBonus = defencePoints.Value;
            }

            if (attackPoints.HasValue)
            {
                card.AttackBonus = attackPoints.Value;
            }

            if (healthPoints.HasValue)
            {
                card.HealthBonus = healthPoints.Value;
            }

            await _cardRepository.SaveChangesAsync();

            _waifuService.DeleteCardImageIfExist(card);

            _cacheManager.ExpireTag(CacheKeys.User(card.GameDeckId), CacheKeys.Users);

            embed = $"Utworzono: {card.GetString(false, false, true)}.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("sc"), Priority(1)]
        [Summary("zmienia SC użytkownika o podaną wartość")]
        [Remarks("Sniku 10000")]
        public async Task ChangeUserScAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba SC")] long amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.ScCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var content = $"{user.Mention} ma teraz {databaseUser.ScCount} SC".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("ac"), Priority(1)]
        [Summary("zmienia AC użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserAcAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba AC")] long amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.AcCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{user.Mention} ma teraz {databaseUser.AcCount} AC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("tc"), Priority(1)]
        [Summary("zmienia TC użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserTcAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba TC")] long amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.TcCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{user.Mention} ma teraz {databaseUser.TcCount} TC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("pc"), Priority(1)]
        [Summary("zmienia PC użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserPcAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba PC")] long amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.GameDeck.PVPCoins += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{user.Mention} ma teraz {databaseUser.GameDeck.PVPCoins} PC".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ct"), Priority(1)]
        [Summary("zmienia CT użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserCtAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba CT")] long amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.GameDeck.CTCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{user.Mention} ma teraz {databaseUser.GameDeck.CTCount} CT".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("exp"), Priority(1)]
        [Summary("zmienia punkty doświadczenia użytkownika o podaną wartość")]
        [Remarks("User 10000")]
        public async Task ChangeUserExpAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba punktów doświadczenia")] ulong amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.ExperienceCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            await ReplyAsync(embed: $"{user.Mention} ma teraz {databaseUser.ExperienceCount} punktów doświadczenia.".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ost"), Priority(1)]
        [Summary("zmienia liczbę punktów ostrzeżeń")]
        [Remarks("User 10000")]
        public async Task ChangeUserOstAsync(
            [Summary(ParameterInfo.User)] IGuildUser user,
            [Summary("liczba ostrzeżeń")] long amount)
        {
            var databaseUser = await _userRepository.GetUserOrCreateAsync(user.Id);
            databaseUser.WarningsCount += amount;

            await _userRepository.SaveChangesAsync();

            _cacheManager.ExpireTag(CacheKeys.User(databaseUser.Id), CacheKeys.Users);

            var content = $"{user.Mention} ma teraz {databaseUser.WarningsCount} punktów ostrzeżeń.".ToEmbedMessage(EMType.Success).Build();
            await ReplyAsync(embed: content);
        }

        [Command("sime"), Priority(1)]
        [Summary("symuluje wyprawę daną kartą")]
        [Remarks("12312 n")]
        public async Task SimulateExpeditionAsync(
            [Summary(ParameterInfo.WID)] ulong wid,
            [Summary(ParameterInfo.ExpeditionCardType)] ExpeditionCardType expedition = ExpeditionCardType.None,
            [Summary(ParameterInfo.Duration)] TimeSpan? duration = null)
        {
            Embed embed;

            if (expedition == ExpeditionCardType.None)
            {
                embed = $"Nie podano typu ekspedycji"
                   .ToEmbedMessage(EMType.Error)
                   .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            var databaseUser = await _userRepository.GetUserAndDontTrackAsync(Context.User.Id);
            var card = databaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (card == null)
            {
                embed = Strings.CardNotFound
                    .ToEmbedMessage(EMType.Error)
                    .Build();
                await ReplyAsync(embed: embed);
                return;
            }

            if (duration.HasValue)
            {
                card.ExpeditionDate = _systemClock.UtcNow - duration.Value;
            }

            card.Expedition = expedition;
            var message = _waifuService.EndExpedition(databaseUser, card, true);
            var cardSummary = card.GetString(false, false, true);

            embed = $"Karta {cardSummary} wróciła z {expedition.GetName("ej")} wyprawy!\n\n{message}"
                .ToEmbedMessage(EMType.Success)
                .Build();

            await ReplyAsync(embed: embed);
        }

        [Command("kill", RunMode = RunMode.Async)]
        [Summary("wyłącza bota")]
        [Remarks("")]
        public async Task TurnOffAsync()
        {
            await ReplyAsync(embed: "To dobry czas by umrzeć.".ToEmbedMessage(EMType.Bot).Build());

            await _discordClientAccessor.LogoutAsync();
            await _taskManager.Delay(TimeSpan.FromMilliseconds(1500));
            _applicationLifetime.StopApplication();
        }

        [Command("update", RunMode = RunMode.Async)]
        [Summary("wyłącza bota z kodem 200")]
        [Remarks("")]
        public async Task TurnOffWithUpdateAsync()
        {
            await ReplyAsync(embed: "To już czas?".ToEmbedMessage(EMType.Bot).Build());

            await _discordClientAccessor.LogoutAsync();

            using var it = _fileSystem.Create(Placeholders.UpdateNow);
            await _taskManager.Delay(TimeSpan.FromMilliseconds(1500));
            _applicationLifetime.StopApplication();
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
            Embed embed;

            if (serverConfig.Any())
            {
                embed = $"**RMC:**\n{string.Join("\n\n", serverConfig)}".ElipseTrimToLength(1900).ToEmbedMessage(EMType.Bot).Build();
                await ReplyAsync(embed: embed);
                return;
            }

            embed = $"**RMC:**\n\nBrak.".ToEmbedMessage(EMType.Bot).Build();
            await ReplyAsync(embed: embed);
        }

        [Command("mrmconfig")]
        [Summary("zmienia istniejący wpis w konfiguracji powiadomień w odniesieniu do serwera, lub tworzy nowy gdy go nie ma")]
        [Remarks("News 1232321314 1232412323 tak")]
        public async Task ChangeRMConfigAsync(
            [Summary("typ wpisu")] RichMessageType type,
            [Summary("id kanału")] ulong channelId,
            [Summary(ParameterInfo.Role)] ulong roleId,
            [Summary("czy zapisać?")] bool save = false)
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

            await ReplyAsync(embed: "Wpis został zmodyfikowany!".ToEmbedMessage(EMType.Success).Build());
        }

        [Command("ignore"), Priority(1)]
        [Summary("dodaje serwer do ignorowanych lub usuwa go z listy")]
        [Remarks("News 1232321314 1232412323 tak")]
        public async Task IgnoreServerAsync()
        {
            var config = _config.Value;
            var guildId = Context.Guild.Id;

            Embed embed;

            if (config.Discord.BlacklistedGuilds.Contains(guildId))
            {
                embed = "Serwer został usunięty z czarnej listy.".ToEmbedMessage(EMType.Success).Build();

                await _config.UpdateAsync(opt =>
                {
                    opt.Discord.BlacklistedGuilds.Remove(guildId);
                });
            }
            else
            {
                embed = "Serwer został dodany do czarnej listy.".ToEmbedMessage(EMType.Success).Build();

                await _config.UpdateAsync(opt =>
                {
                    opt.Discord.BlacklistedGuilds.Add(guildId);
                });
            }

            await ReplyAsync(embed: embed);
        }

        [Command("pomoc", RunMode = RunMode.Async)]
        [Alias("help", "h")]
        [Summary("wypisuje polecenia")]
        [Remarks("kasuj")]
        public async Task SendHelpAsync(
            [Summary(ParameterInfo.CommandOptional)][Remainder] string? command = null)
        {
            if (command == null)
            {
                var message = _helperService.GivePrivateHelp(PrivateModules.Debug);
                await ReplyAsync(message);

                return;
            }

            try
            {
                var prefix = _config.Value.Discord.Prefix;
                var guild = Context.Guild;

                if (guild != null)
                {
                    var guildConfig = await _guildConfigRepository.GetCachedById(guild.Id);

                    if (guildConfig?.Prefix != null)
                    {
                        prefix = guildConfig.Prefix;
                    }
                }

                var message = _helperService.GiveHelpAboutPrivateCommand(PrivateModules.Debug, command, prefix);
                await ReplyAsync(message);
            }
            catch (Exception ex)
            {
                await ReplyAsync(embed: ex.Message.ToEmbedMessage(EMType.Error).Build());
            }
        }
    }
}
