using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Config;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Sanakan.Web.Models;
using Sanakan.Web.Resources;
using static Sanakan.Web.ResponseExtensions;
using Sanakan.ShindenApi.Utilities;
using Microsoft.Extensions.Options;
using Sanakan.Configuration;
using Sanakan.Common.Cache;
using System.Collections.Concurrent;
using Sanakan.TaskQueue.Messages;
using Sanakan.Game.Models;
using Sanakan.Common.Configuration;
using Sanakan.TaskQueue;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class WaifuController : ControllerBase
    {
        private readonly IShindenClient _shindenClient;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly IOptionsMonitor<ApiConfiguration> _config;
        private readonly IWaifuService _waifuService;
        private readonly IFileSystem _fileSystem;
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IUserContext _userContext;
        private readonly ICacheManager _cacheManager;
        private readonly IJwtBuilder _jwtBuilder;

        public WaifuController(
            IShindenClient shindenClient,
            IBlockingPriorityQueue blockingPriorityQueue,
            IOptionsMonitor<ApiConfiguration> config,
            IWaifuService waifuService,
            IFileSystem fileSystem,
            IUserRepository userRepository,
            ICardRepository cardRepository,
            IUserContext userContext,
            ICacheManager cacheManager,
            IJwtBuilder jwtBuilder)
        {
            _shindenClient = shindenClient;
            _blockingPriorityQueue = blockingPriorityQueue;
            _config = config;
            _waifuService = waifuService;
            _fileSystem = fileSystem;
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _userContext = userContext;
            _cacheManager = cacheManager;
            _jwtBuilder = jwtBuilder;
        }

        /// <summary>
        /// Gets the list of user which contain character card.
        /// </summary>
        /// <param name="id">The shinden user identifier.</param>
        [HttpGet("users/owning/character/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<ulong>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersOwningCharacterCardAsync(ulong id)
        {
            var shindenIds = await _userRepository.GetUserShindenIdsByHavingCharacterAsync(id);

            if (shindenIds.Any())
            {
                return Ok(shindenIds);
            }

            return ShindenNotFound("Users not found");
        }

        /// <summary>
        /// Gets the list of cards which user has.
        /// </summary>
        /// <param name="id">The Shinden user identifier</param>
        [HttpGet("user/{id}/cards"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserCardsAsync(ulong shindenUserId)
        {
            var user = await _cardRepository.GetUserCardsAsync(shindenUserId);

            if (user == null)
            {
                return ShindenNotFound("User not found");
            }

            var result = user.GameDeck.Cards;

            return Ok(result);
        }

        /// <summary>
        /// Pobiera x kart z przefiltrowanej listy użytkownika
        /// </summary>
        /// <param name="id">The user identifier</param>
        /// <param name="offset">offset</param>
        /// <param name="count">number of cards to take</param>
        /// <param name="filter">filtry listy</param>
        [HttpPost("user/{id}/cards/{offset}/{count}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FilteredCards), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersCardsByShindenIdWithOffsetAndFilterAsync(
            ulong id,
            uint offset,
            uint count,
            [FromBody]CardsQueryFilter filter)
        {
            var user = await _userRepository.GetByShindenIdAsync(id, new UserQueryOptions
            {
                IncludeGameDeck = true,
            });

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var cards = await _cardRepository.GetAsync(user.GameDeck.Id, filter);

            var result = new FilteredCards
            { 
                TotalCards = cards.Count,
                Cards = cards.Skip((int)offset).Take((int)count).ToView(),
            };

            return Ok(result);
        }

        /// <summary>
        /// Pobiera x kart z listy użytkownika
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        /// <param name="offset">przesunięcie</param>
        /// <param name="count">liczba kart</param>
        [HttpGet("user/{id}/cards/{offset}/{count}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<CardFinalView>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersCardsByShindenIdWithOffsetAsync(
            ulong id, int offset, int count)
        {
            var user = await _userRepository.GetByShindenIdAsync(id, new UserQueryOptions
            {
                IncludeGameDeck = true,
            });

            if (user == null)
            {
                return ShindenNotFound("User not found");
            }

            var cards = await _cardRepository.GetByGameDeckIdAsync(user.GameDeck.Id, offset, count);
            var result = cards.ToView();

            return Ok(result);
        }

        /// <summary>
        /// Pobiera surową listę życzeń użtykownika
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        /// <returns>lista życzeń</returns>
        [HttpGet("user/shinden/{id}/wishlist/raw")]
        [ProducesResponseType(typeof(IEnumerable<WishlistObject>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUsersRawWishlistByShindenIdAsync(ulong id)
        {
            var user = await _userRepository.GetByShindenIdAsync(id, new UserQueryOptions
            {
                IncludeGameDeck = true,
                IncludeWishes = true,
            });

            if (user == null)
            {
                return ShindenNotFound("User not found");
            }

            if (user.GameDeck.WishlistIsPrivate)
            {
                return ShindenUnauthorized("User wishlist is private");
            }

            var result = user.GameDeck.Wishes;

            return Ok(result);
        }

        /// <summary>
        /// Pobiera profil użytkownika
        /// </summary>
        /// <param name="id">The user identifier in Shinden.</param>
        [HttpGet("user/{id}/profile")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserWaifuProfileAsync(ulong id)
        {
            var user = await _userRepository.GetUserWaifuProfileAsync(id);

            if (user == null)
            {
                return ShindenNotFound("User not found");
            }

            var tagList = new List<string>();
            var tags = user.GameDeck.Cards
                .Where(x => x.TagList != null)
                .Select(x => x.TagList.Select(c => c.Name));
            
            foreach (var tag in tags)
            {
                tagList.AddRange(tag);
            }

            var cardCount = new Dictionary<string, long>
                {
                    {Rarity.SSS.ToString(), user.GameDeck.Cards.Count(x => x.Rarity == Rarity.SSS)},
                    {Rarity.SS.ToString(),  user.GameDeck.Cards.Count(x => x.Rarity == Rarity.SS)},
                    {Rarity.S.ToString(),   user.GameDeck.Cards.Count(x => x.Rarity == Rarity.S)},
                    {Rarity.A.ToString(),   user.GameDeck.Cards.Count(x => x.Rarity == Rarity.A)},
                    {Rarity.B.ToString(),   user.GameDeck.Cards.Count(x => x.Rarity == Rarity.B)},
                    {Rarity.C.ToString(),   user.GameDeck.Cards.Count(x => x.Rarity == Rarity.C)},
                    {Rarity.D.ToString(),   user.GameDeck.Cards.Count(x => x.Rarity == Rarity.D)},
                    {Rarity.E.ToString(),   user.GameDeck.Cards.Count(x => x.Rarity == Rarity.E)},
                    {"max",                 user.GameDeck.MaxNumberOfCards},
                    {"total",               user.GameDeck.Cards.Count}
                };

            var wallet = new Dictionary<string, long>
                {
                    {"PC", user.GameDeck.PVPCoins},
                    {"CT", user.GameDeck.CTCount},
                    {"AC", user.AcCount},
                    {"TC", user.TcCount},
                    {"SC", user.ScCount},
                };

            var result = new UserSiteProfile()
            {
                Wallet = wallet,
                CardsCount = cardCount,
                Karma = user.GameDeck.Karma,
                TagList = tagList.Distinct().ToList(),
                UserTitle = user.GameDeck.GetUserNameStatus(),
                ForegroundColor = user.GameDeck.ForegroundColor,
                ForegroundPosition = user.GameDeck.ForegroundPosition,
                BackgroundPosition = user.GameDeck.BackgroundPosition,
                ExchangeConditions = user.GameDeck.ExchangeConditions,
                BackgroundImageUrl = user.GameDeck.BackgroundImageUrl,
                ForegroundImageUrl = user.GameDeck.ForegroundImageUrl,
                Expeditions = user.GameDeck.Cards.Where(x => x.Expedition != ExpeditionCardType.None).ToExpeditionView(user.GameDeck.Karma),
                Waifu = user.GameDeck.Cards.Where(x => x.CharacterId == user.GameDeck
                .FavouriteWaifuId)
                .OrderBy(x => x.Rarity).ThenByDescending(x => x.Quality).FirstOrDefault().ToView(),
                Gallery = user.GameDeck.
                Cards.Where(x => x.HasTag("galeria"))
                .Take(user.GameDeck.CardsInGallery)
                .OrderBy(x => x.Rarity)
                .ThenByDescending(x => x.Quality).ToView()
            };

            return Ok(result);
        }

        /// <summary>
        /// Replaces character ids in cards.
        /// </summary>
        /// <param name="oldId">The character id from Shinden database which was deleted.</param>
        /// <param name="newId">The new character id from Shinden database.</param>
        [HttpPost("character/repair/{oldCharacterId}/{newCharacterId}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RepairCardsAsync(ulong oldCharacterId, ulong newCharacterId)
        {
            var characterResult = await _shindenClient.GetCharacterInfoAsync(newCharacterId);

            if (characterResult.Value == null)
            {
                return new ObjectResult("New character ID is invalid!")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }

            var message = new ReplaceCharacterIdsInCardMessage
            {
                OldCharacterId = oldCharacterId,
                NewCharacterId = newCharacterId
            };

            _blockingPriorityQueue.TryEnqueue(message);

            //var exe = new Executable($"api-repair oc{oldId} c{newId}", new Task<Task>(async () =>
            //{
              
            //}), Priority.High);

            //await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));

            return ShindenOk("Success");
        }

        /// <summary>
        /// Updates card information for given character.
        /// </summary>
        /// <param name="characterId">The character identifier in Shinden.</param>
        /// <param name="newData">New card information.</param>
        [HttpPost("cards/character/{characterId}/update"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCardInfoAsync(
            ulong characterId,
            [FromBody]CharacterCardInfoUpdate model)
        {

            _blockingPriorityQueue.TryEnqueue(new UpdateCardMessage
            {
                CharacterId = characterId,
                ImageUrl = model.ImageUrl,
                CharacterName = model.CharacterName,
                CardSeriesTitle = model.CardSeriesTitle,
            });

            return ShindenOk("Started!");
        }

        /// <summary>
        /// Generates the renewed card of given character.
        /// </summary>
        /// <param name="characterId">The character identifier from Shinden database.</param>
        [HttpPost("users/make/character/{characterId}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateCharacterCardAsync(ulong characterId)
        {
            var characterResult = await _shindenClient.GetCharacterInfoAsync(characterId);

            if (characterResult.Value == null)
            {
                return ShindenNotFound("Character not found!");
            }

            var characterInfo = characterResult.Value;
            var pictureUrl = UrlHelpers.GetPersonPictureURL(characterInfo.PictureId);
            var hasImage = pictureUrl != UrlHelpers.GetPlaceholderImageURL();

            if (!hasImage)
            {
                return ShindenMethodNotAllowed("There is no character image!");
            }

            _blockingPriorityQueue.TryEnqueue(new UpdateCardPictureMessage
            {
                CharacterId = characterId,
                PictureId = characterInfo.PictureId.Value,
            });

            return ShindenOk("Started!");
        }

        /// <summary>
        /// Pobiera listę życzeń użytkownika
        /// </summary>
        /// <param name="id">id użytkownika discorda</param>
        [HttpGet("user/discord/{id}/wishlist"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserWishlistAsync(ulong id)
        {
            var user = await _userRepository.GetCachedFullUserAsync(id);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            if (user.GameDeck.Wishes.Count < 1)
            {
                return ShindenNotFound("Wishlist not found!");
            }

            var characterIds = user.GameDeck.GetCharactersWishList();
            var titleIds = user.GameDeck.GetTitlesWishList();
            var cardsId = user.GameDeck.GetCardsWishList();

            var allCards = new List<Card>();

            if (cardsId != null)
            {
                var cards = await _cardRepository.GetByIdsAsync(cardsId.ToArray(), new CardQueryOptions
                {
                    IncludeTagList = true,
                    AsNoTracking = true,
                });

                allCards.AddRange(cards);
            }

            var result = await _waifuService.GetCardsFromWishlist(
                cardsId,
                characterIds,
                titleIds,
                allCards,
                user.GameDeck.Cards);

            return Ok(result);
        }

        /// <summary>
        /// Pobiera listę życzeń użytkownika
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        [HttpGet("user/shinden/{id}/wishlist"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShindenUserWishlistAsync(ulong id)
        {
            var user = await _userRepository.GetCachedFullUserByShindenIdAsync(id);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            if (user.GameDeck.Wishes.Count < 1)
            {
                return ShindenNotFound("Wishlist not found!");
            }

            var characterIds = user.GameDeck.GetCharactersWishList();
            var titleIds = user.GameDeck.GetTitlesWishList();
            var cardsId = user.GameDeck.GetCardsWishList();

            var allCards = new List<Card>();

            if (cardsId != null)
            {
                var cards = await _cardRepository.GetByIdsAsync(
                    cardsId.ToArray(),
                    new CardQueryOptions
                    {
                        IncludeTagList = true,
                        AsNoTracking = true,
                    });
             
                allCards.AddRange(cards);
            }

            var result = await _waifuService.GetCardsFromWishlist(
                cardsId,
                characterIds,
                titleIds,
                allCards,
                user.GameDeck.Cards);
            return Ok(result);
        }

        /// <summary>
        /// Pobiera liste kart z danym tagiem
        /// </summary>
        /// <param name="tag">tag na karcie</param>
        [HttpGet("cards/tag/{tag}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCardsWithTagAsync(string tag)
        {
            var result = await _cardRepository.GetCardsWithTagAsync(tag);

            return Ok(result);
        }

        /// <summary>
        /// Tries to generate an image if it doesnt exist.
        /// Wymusza na bocie wygenerowanie obrazka jeśli nie istnieje
        /// </summary>
        /// <param name="id">id karty (wid)</param>
        [HttpGet("card/{id}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCardAsync(ulong id)
        {
            if (!_fileSystem.Exists($"{Paths.CardsMiniatures}/{id}.png") 
                || !_fileSystem.Exists($"{Paths.Cards}/{id}.png") 
                || !_fileSystem.Exists($"{Paths.CardsInProfiles}/{id}.png"))
            {
                var card = await _cardRepository.GetByIdAsync(id);

                if (card == null)
                {
                    return ShindenNotFound(Strings.CardNotFound);
                }
                
                _waifuService.DeleteCardImageIfExist(card);
                var cardImage = await _waifuService.GenerateAndSaveCardAsync(card);
                var stream = _fileSystem.OpenRead(cardImage);

                return File(stream, "image/png");
            }

            return ShindenForbidden("Card already exist!");
        }

        /// <summary>
        /// Gives bundle of cards for given Discord user.
        /// </summary>
        /// <param name="discordUserId">The user identifier in Discord.</param>
        /// <param name="boosterPacks">The bundle model.</param>
        /// <returns>użytkownik bota</returns>
        [HttpPost("discord/{discordUserId}/boosterpack"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GiveUserAPacksAsync(
            ulong discordUserId,
            [FromBody] List<CardBoosterPack> boosterPacks)
        {
            if (boosterPacks?.Count < 1)
            {
                return ShindenInternalServerError(Strings.ModelIsInvalid);
            }

            var packs = new List<BoosterPack>();

            foreach (var pack in boosterPacks)
            {
                var rPack = pack.ToRealPack();
                if (rPack != null)
                {
                    packs.Add(rPack);
                }
            }

            if (packs.Count < 1)
            {
                return ShindenInternalServerError("Data is Invalid");
            }

            var user = await _userRepository.GetCachedFullUserAsync(discordUserId);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            _blockingPriorityQueue.TryEnqueue(new GivesCardsMessage
            {
                DiscordUserId = discordUserId,
                BoosterPacks = boosterPacks,
            });

            //var exe = new Executable($"api-packet u{id}", new Task<Task>(async () =>
            //{
            //    var botUser = await _userRepository.GetUserOrCreateAsync(id);

            //    foreach (var pack in packs)
            //    {
            //        botUser.GameDeck.BoosterPacks.Add(pack);
            //    }

            //    await _userRepository.SaveChangesAsync();

            //    _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });
            //}));

            //await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
            return ShindenOk("Boosterpack added!");
        }

        /// <summary>
        /// Gives bundle of cards to given user.
        /// </summary>
        /// <param name="shindenUserId">The user identifier in Shinden.</param>
        /// <param name="boosterPacks">The bundle of cards model.</param>
        /// <response code="404">User not found</response>
        /// <response code="500">Model is Invalid</response>
        [HttpPost("shinden/{id}/boosterpack"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GiveShindenUserAPacksAsync(
            ulong shindenUserId, [FromBody]List<CardBoosterPack> boosterPacks)
        {
            if (boosterPacks?.Count < 1)
            {
                return ShindenInternalServerError(Strings.ModelIsInvalid);
            }

            var packs = new List<BoosterPack>();
            foreach (var pack in boosterPacks)
            {
                var rPack = pack.ToRealPack();
                
                if (rPack != null)
                {
                    packs.Add(rPack);
                }
            }

            if (packs.Count < 1)
            {
                return ShindenInternalServerError("Data is Invalid");
            }

            var user = await _userRepository.GetCachedFullUserByShindenIdAsync(shindenUserId);
            
            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var discordUserId = user.Id;

            _blockingPriorityQueue.TryEnqueue(new OpenCardsMessage
            {
                DiscordUserId = discordUserId,
            });

            //var exe = new Executable($"api-packet u{discordId}", new Task<Task>(async () =>
            //{
            //    var botUser = await _userRepository.GetUserOrCreateAsync(discordId);

            //    foreach (var pack in packs)
            //    {
            //        botUser.GameDeck.BoosterPacks.Add(pack);
            //    }

            //    await _userRepository.SaveChangesAsync();

            //    _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });
            //}));

            //await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));

            TokenData tokenData = null;
            
            if (_userContext.HasWebpageClaim())
            {
                tokenData = _jwtBuilder.Build(_config.CurrentValue.UserWithTokenExpiry);
            }

            var result = new UserWithToken()
            {
                Expire = tokenData?.Expire,
                Token = tokenData?.Token,
                User = user,
            };

            return Ok(result);
        }

        /// <summary>
        /// Opens packet and add cards to user collection.
        /// </summary>
        /// <param name="shindenUserId">The Shinden user identifier</param>
        /// <param name="boosterPacks">Packet model</param>
        [HttpPost("shinden/{id}/boosterpack/open"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GiveShindenUserAPacksAndOpenAsync(
            ulong shindenUserId, [FromBody]List<CardBoosterPack> boosterPacks)
        {
            if (boosterPacks?.Count < 1)
            {
                return ShindenInternalServerError(Strings.ModelIsInvalid);
            }

            var packs = new List<BoosterPack>();

            foreach (var pack in boosterPacks)
            {
                var rPack = pack.ToRealPack();
                if (rPack != null)
                {
                    packs.Add(rPack);
                }
            }

            if (packs.Count < 1)
            {
                return ShindenInternalServerError("Data is Invalid");
            }

            var user = await _userRepository.GetByShindenIdAsync(shindenUserId, new UserQueryOptions
            {
                IncludeGameDeck = true,
                IncludeCards = true,
            });

            var gameDeck = user.GameDeck;

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }
            if (gameDeck.Cards.Count + packs.Sum(x => x.CardCount) > gameDeck.MaxNumberOfCards)
            {
                return ShindenNotAcceptable(Strings.NoSpaceInDeck);
            }

            var discordUserId = user.Id;

            var cards = new List<Card>();
            foreach (var pack in packs)
            {
                cards.AddRange(await _waifuService.OpenBoosterPackAsync(null, pack));
            }

            _blockingPriorityQueue.TryEnqueue(new OpenCardsMessage
            {
                DiscordUserId = discordUserId,
                Cards = cards,
            });

            //var exe = new Executable($"api-packet-open u{discordId}", new Task<Task>(async () =>
            //{
            //    var botUser = await _userRepository.GetUserOrCreateAsync(discordId);

            //    botUser.Stats.OpenedBoosterPacks += packs.Count;

            //    foreach (var card in cards)
            //    {
            //        card.Affection += botUser.GameDeck.AffectionFromKarma();
            //        card.FirstIdOwner = botUser.Id;

            //        botUser.GameDeck.Cards.Add(card);
            //        botUser.GameDeck.RemoveCharacterFromWishList(card.CharacterId);
            //    }

            //    await _userRepository.SaveChangesAsync();

            //    _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });
            //}));

            //if (!await _executor.TryAdd(exe, TimeSpan.FromSeconds(1)))
            //{
            //    return ShindenServiceUnavailable("Command queue is full");
            //}

            //await exe.WaitAsync();

            return Ok(cards);
        }

        /// <summary>
        /// Opens user packet (requires authorization)
        /// </summary>
        /// <param name="packNumber">numer pakietu</param>
        [HttpPost("boosterpack/open/{packNumber}"), Authorize(Policy = AuthorizePolicies.Player)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ObjectResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> OpenAPackAsync(int packNumber)
        {
            var discordId = _userContext.DiscordId;

            if (!discordId.HasValue)
            {
                return new ObjectResult("The appropriate claim was not found")
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                };
            }

            var bPackName = "";
            var cards = new List<Card>();
            var botUserCh = await _userRepository.GetCachedFullUserAsync(discordId.Value);

            if (botUserCh == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            if (botUserCh.GameDeck.BoosterPacks.Count < packNumber || packNumber <= 0)
            {
                return ShindenNotFound("Boosterpack not found!");
            }

            var pack = botUserCh.GameDeck.BoosterPacks.ToArray()[packNumber - 1];

            if (botUserCh.GameDeck.Cards.Count + pack.CardCount > botUserCh.GameDeck.MaxNumberOfCards)
            {
                return ShindenNotAcceptable("User has no space left in deck!");
            }

            cards = await _waifuService.OpenBoosterPackAsync(null, pack);
            bPackName = pack.Name;

            _blockingPriorityQueue.TryEnqueue(new OpenCardsMessage
            {
                DiscordUserId = discordId.Value,
                Cards = cards,
            });

            //var exe = new Executable($"api-packet-open u{discordId}", new Task<Task>(async () =>
            //{
            //    var botUser = await _userRepository.GetUserOrCreateAsync(discordId.Value);

            //    var bPack = botUser.GameDeck.BoosterPacks.ToArray()[packNumber - 1];
            //    if (bPack?.Name != bPackName)
            //    {
            //        return ShindenInternalServerError("Boosterpack already opened!");
            //    }

            //    botUser.GameDeck.BoosterPacks.Remove(bPack);

            //    if (bPack.CardSourceFromPack == CardSource.Activity || bPack.CardSourceFromPack == CardSource.Migration)
            //    {
            //        botUser.Stats.OpenedBoosterPacksActivity += 1;
            //    }
            //    else
            //    {
            //        botUser.Stats.OpenedBoosterPacks += 1;
            //    }

            //    foreach (var card in cards)
            //    {
            //        card.Affection += botUser.GameDeck.AffectionFromKarma();
            //        card.FirstIdOwner = botUser.Id;

            //        botUser.GameDeck.Cards.Add(card);
            //        botUser.GameDeck.RemoveCharacterFromWishList(card.Character);
            //    }

            //    await _repository.SaveChangesAsync();

            //    _cacheManager.ExpireTag(new string[] { $"user-{botUser.Id}", "users" });
            //}));

            //await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));

            //exe.Wait();

            return Ok(cards);
        }

        /// <summary>
        /// Activates or deactivates card ( authorization required )
        /// </summary>
        /// <param name="wid">id karty</param>
        [HttpPut("deck/toggle/card/{wid}"), Authorize(Policy = AuthorizePolicies.Player)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ToggleCardStatusAsync(ulong wid)
        {
            var discordId = _userContext.DiscordId;

            if (!discordId.HasValue)
            {
                return ShindenForbidden("The appropriate claim was not found");
            }

            var botUserCh = await _userRepository.GetCachedFullUserAsync(discordId.Value);

            if (botUserCh == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var thisCardCh = botUserCh.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (thisCardCh == null)
            {
                return ShindenNotFound(Strings.CardNotFound);
            }

            if (thisCardCh.InCage)
            {
                return ShindenForbidden("Card is in cage!");
            }

            _blockingPriorityQueue.TryEnqueue(new ToggleCardMessage
            {
                DiscordUserId = discordId.Value,
                WId = wid,
            });

            return ShindenOk("Card status toggled");
        }
    }
}