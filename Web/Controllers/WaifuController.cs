using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Cache;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Utilities;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Extensions;
using Sanakan.Web.Models;
using Sanakan.Web.Resources;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static Sanakan.Web.ResponseExtensions;

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
        /// Gets the list of user identifiers which contain character card.
        /// </summary>
        /// <param name="id">The shinden user identifier.</param>
        [HttpGet("users/owning/character/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<ulong>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserIdsOwningCharacterCardAsync(ulong id)
        {
            var shindenIds = await _userRepository.GetUserShindenIdsByHavingCharacterAsync(id);

            if (shindenIds.Any())
            {
                return Ok(shindenIds);
            }

            return ShindenNotFound(Strings.UsersNotFound);
        }

        /// <summary>
        /// Gets the list of cards which user has.
        /// </summary>
        /// <param name="shindenUserId">The Shinden user identifier</param>
        [HttpGet("user/{shindenUserId}/cards"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserCardsAsync(ulong shindenUserId)
        {
            var user = await _cardRepository.GetUserCardsAsync(shindenUserId);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var result = user.GameDeck.Cards;

            return Ok(result);
        }

        /// <summary>
        /// Gets cards from from user collection.
        /// </summary>
        /// <param name="shindenUserId">The Shinden user identifier.</param>
        /// <param name="offset">offset.</param>
        /// <param name="take">number of cards to take.</param>
        /// <param name="filter">The filter criteria.</param>
        [HttpPost("user/{shindenUserId}/cards/{offset}/{take}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(FilteredCards), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersCardsByShindenIdWithOffsetAndFilterAsync(
            ulong shindenUserId,
            int offset,
            int take,
            [FromBody] CardsQueryFilter filter)
        {
            var user = await _userRepository.GetByShindenIdAsync(shindenUserId, new UserQueryOptions
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
                Cards = cards.Skip(offset).Take(take).ToView(),
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets cards from from user collection.
        /// </summary>
        /// <param name="shindenUserId">The Shinden user identifier.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="take">The number of cards to take.</param>
        [HttpGet("user/{shindenUserId}/cards/{offset}/{take}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<CardFinalView>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsersCardsByShindenIdWithOffsetAsync(
            ulong shindenUserId,
            int offset,
            int take)
        {
            var user = await _userRepository.GetByShindenIdAsync(shindenUserId, new UserQueryOptions
            {
                IncludeGameDeck = true,
            });

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var cards = await _cardRepository.GetByGameDeckIdAsync(user.GameDeck.Id, offset, take);
            var result = cards.ToView();

            return Ok(result);
        }

        /// <summary>
        /// Gets the wishlist for given user.
        /// </summary>
        /// <param name="id">The shinden user identifier.</param>
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
                return ShindenNotFound(Strings.UserNotFound);
            }

            if (user.GameDeck.WishlistIsPrivate)
            {
                return ShindenUnauthorized("User wishlist is private");
            }

            var result = user.GameDeck.Wishes;

            return Ok(result);
        }

        /// <summary>
        /// Gets user profile.
        /// </summary>
        /// <param name="shindenUserId">The user identifier in Shinden.</param>
        [HttpGet("user/{shindenUserId}/profile")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserWaifuProfileAsync(ulong shindenUserId)
        {
            var user = await _userRepository.GetUserWaifuProfileAsync(shindenUserId);

            if (user == null)
            {
                return ShindenNotFound("User not found");
            }

            var gameDeck = user.GameDeck;

            var tagList = gameDeck.Cards
                .SelectMany(x => x.Tags.Select(c => c.Name))
                .Distinct()
                .ToList();

            var cards = gameDeck.Cards;
            var cardCount = new Dictionary<string, long>();
            foreach (var card in cards)
            {
                var rarity = card.Rarity.ToString();
                if (!cardCount.ContainsKey(rarity))
                {
                    cardCount[rarity] = 0;
                }
                cardCount[rarity]++;
            }

            cardCount["max"] = gameDeck.MaxNumberOfCards;
            cardCount["total"] = cards.Count;

            var wallet = new Dictionary<string, long>
                {
                    {"PC", gameDeck.PVPCoins},
                    {"CT", gameDeck.CTCount},
                    {"AC", user.AcCount},
                    {"TC", user.TcCount},
                    {"SC", user.ScCount},
                };

            var waifuCard = cards
                .Where(x => x.CharacterId == gameDeck.FavouriteWaifuId)
                .OrderBy(x => x.Rarity)
                    .ThenByDescending(x => x.Quality)
                .FirstOrDefault();

            var waifu = waifuCard == null ? null : new CardFinalView(waifuCard);

            var gallery = gameDeck.Cards
                .Where(x => x.HasTag(Tags.Gallery))
                .Take(gameDeck.CardsInGalleryCount)
                .OrderBy(x => x.Rarity)
                .ThenByDescending(x => x.Quality).ToView();

            var result = new UserSiteProfile()
            {
                Wallet = wallet,
                CardsCount = cardCount,
                Karma = gameDeck.Karma,
                TagList = tagList,
                UserTitle = gameDeck.GetUserNameStatus(),
                ForegroundColor = gameDeck.ForegroundColor,
                ForegroundPosition = gameDeck.ForegroundPosition,
                BackgroundPosition = gameDeck.BackgroundPosition,
                ExchangeConditions = gameDeck.ExchangeConditions!,
                BackgroundImageUrl = gameDeck.BackgroundImageUrl?.ToString(),
                ForegroundImageUrl = gameDeck.ForegroundImageUrl?.ToString(),
                Expeditions = cards.Where(x => x.Expedition != ExpeditionCardType.None).ToExpeditionView(gameDeck.Karma),
                Waifu = waifu!,
                Gallery = gallery,
            };

            return Ok(result);
        }

        /// <summary>
        /// Replaces character ids in cards.
        /// </summary>
        /// <param name="oldCharacterId">The character id from Shinden database which was deleted.</param>
        /// <param name="newCharacterId">The new character id from Shinden database.</param>
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

            return ShindenOk("Success");
        }

        /// <summary>
        /// Updates card information for given character.
        /// </summary>
        /// <param name="characterId">The character identifier in Shinden.</param>
        /// <param name="model">New card information.</param>
        [HttpPost("cards/character/{characterId}/update"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public IActionResult UpdateCardInfo(
            ulong characterId,
            [FromBody] CharacterCardInfoUpdate model)
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
                PictureId = characterInfo.PictureId!.Value,
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
            var gameDeck = user.GameDeck;

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            if (gameDeck.Wishes.Count < 1)
            {
                return ShindenNotFound("Wishlist not found!");
            }

            var characterIds = gameDeck.GetCharactersWishList();
            var titleIds = gameDeck.GetTitlesWishList();
            var cardIds = gameDeck.GetCardsWishList();

            var allCards = new List<Card>();

            if (cardIds.Any())
            {
                var cards = await _cardRepository.GetByIdsAsync(cardIds, new CardQueryOptions
                {
                    IncludeTagList = true,
                    AsNoTracking = true,
                });

                allCards.AddRange(cards);
            }

            var result = await _waifuService.GetCardsFromWishlist(
                cardIds,
                characterIds,
                titleIds,
                allCards,
                user.GameDeck.Cards);

            return Ok(result);
        }

        /// <summary>
        /// Gets the user wishlist.
        /// </summary>
        /// <param name="shindenUserId">The shinden user identifier.</param>
        [HttpGet("user/shinden/{shindenUserId}/wishlist"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Card>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShindenUserWishlistAsync(ulong shindenUserId)
        {
            var user = await _userRepository.GetCachedFullUserByShindenIdAsync(shindenUserId);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var gameDeck = user.GameDeck;

            if (gameDeck.Wishes.Count < 1)
            {
                return ShindenNotFound("Wishlist not found!");
            }

            var characterIds = gameDeck.GetCharactersWishList();
            var titleIds = gameDeck.GetTitlesWishList();
            var cardsId = gameDeck.GetCardsWishList();

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
                cardsId!,
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
        /// </summary>
        /// <param name="cardId">The card identifier.</param>
        [HttpGet("card/{cardId}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCardAsync(ulong cardId)
        {
            if (!_fileSystem.Exists($"{Paths.CardsMiniatures}/{cardId}.png")
                || !_fileSystem.Exists($"{Paths.Cards}/{cardId}.png")
                || !_fileSystem.Exists($"{Paths.CardsInProfiles}/{cardId}.png"))
            {
                var card = await _cardRepository.GetByIdAsync(cardId);

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
        /// Gives bundle of cards to given Discord user.
        /// </summary>
        /// <param name="discordUserId">The user identifier in Discord.</param>
        /// <param name="boosterPacks">The bundle model.</param>
        [HttpPost("discord/{discordUserId}/boosterpack"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GiveUserAPacksAsync(
            ulong discordUserId,
            [FromBody] IEnumerable<CardBoosterPack>? boosterPacks)
        {
            boosterPacks ??= Enumerable.Empty<CardBoosterPack>();

            if (!boosterPacks.Any())
            {
                return ShindenInternalServerError(Strings.ModelIsInvalid);
            }

            var packs = new List<BoosterPack>();

            foreach (var pack in boosterPacks)
            {
                var realPack = pack.ToRealPack();
                if (realPack != null)
                {
                    packs.Add(realPack);
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

            _blockingPriorityQueue.TryEnqueue(new GiveCardsMessage
            {
                DiscordUserId = discordUserId,
                BoosterPacks = packs,
            });

            return ShindenOk("Boosterpack added!");
        }

        /// <summary>
        /// Gives bundle of cards to given user.
        /// </summary>
        /// <param name="shindenUserId">The user identifier in Shinden.</param>
        /// <param name="boosterPacks">The bundle of cards model.</param>
        /// <response code="404">User not found</response>
        /// <response code="500">Model is Invalid</response>
        [HttpPost("shinden/{shindenUserId}/boosterpack"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GiveShindenUserAPacksAsync(
            ulong shindenUserId, [FromBody] IEnumerable<CardBoosterPack> boosterPacks)
        {
            if (boosterPacks.Count() < 1)
            {
                return ShindenInternalServerError(Strings.ModelIsInvalid);
            }

            var packs = new List<BoosterPack>();
            foreach (var pack in boosterPacks)
            {
                var realPack = pack.ToRealPack();

                if (realPack != null)
                {
                    packs.Add(realPack);
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

            _blockingPriorityQueue.TryEnqueue(new GiveCardsMessage
            {
                DiscordUserId = discordUserId,
                BoosterPacks = packs,
            });

            TokenData? tokenData = null;

            if (_userContext.HasWebpageClaim())
            {
                var claims = new[]
                {
                    new Claim(RegisteredNames.DiscordId, user.Id.ToString()),
                    new Claim(RegisteredNames.Player, RegisteredNames.WaifuPlayer),
                };

                tokenData = _jwtBuilder.Build(_config.CurrentValue.Jwt.UserWithTokenExpiry, claims);
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
        /// <param name="shindenUserId">The Shinden user identifier.</param>
        /// <param name="boosterPacks">The list of booster packs.</param>
        [HttpPost("shinden/{shindenUserId}/boosterpack/open"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> GiveShindenUserAPacksAndOpenAsync(
            ulong shindenUserId, [FromBody] IEnumerable<CardBoosterPack> boosterPacks)
        {
            if (boosterPacks.Count() < 1)
            {
                return ShindenInternalServerError(Strings.ModelIsInvalid);
            }

            var packs = new List<BoosterPack>();

            foreach (var pack in boosterPacks)
            {
                var realPack = pack.ToRealPack();
                if (realPack != null)
                {
                    packs.Add(realPack);
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

            var enqueued = _blockingPriorityQueue.TryEnqueue(new GiveBoosterPackMessage
            {
                DiscordUserId = discordUserId,
                Cards = cards,
                PackCount = packs.Count,
            });

            if (!enqueued)
            {
                return ShindenServiceUnavailable("Command queue is full");
            }

            return Ok(cards);
        }

        /// <summary>
        /// Opens user packet.
        /// </summary>
        /// <param name="packNumber">The bundle number.</param>
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
                return new ObjectResult(Strings.ClaimNotFound)
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                };
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(discordId.Value);

            if (databaseUser == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var gameDeck = databaseUser.GameDeck;
            var boosterPacks = gameDeck.BoosterPacks;

            if (boosterPacks.Count < packNumber || packNumber <= 0)
            {
                return ShindenNotFound("Boosterpack not found!");
            }

            var pack = boosterPacks[packNumber - 1];

            if (gameDeck.Cards.Count + pack.CardCount > gameDeck.MaxNumberOfCards)
            {
                return ShindenNotAcceptable("User has no space left in deck!");
            }

            var cards = await _waifuService.OpenBoosterPackAsync(null, pack);
            var boosterPackName = pack.Name;

            databaseUser = await _userRepository.GetUserOrCreateAsync(discordId.Value);
            gameDeck = databaseUser.GameDeck;
            boosterPacks = gameDeck.BoosterPacks;
            var boosterPack = boosterPacks[packNumber - 1];

            if (boosterPack?.Name != boosterPackName)
            {
                return ShindenInternalServerError("Boosterpack already opened!");
            }

            boosterPacks.Remove(boosterPack);

            if (boosterPack.CardSourceFromPack == CardSource.Activity || boosterPack.CardSourceFromPack == CardSource.Migration)
            {
                databaseUser.Stats.OpenedBoosterPacksActivity += 1;
            }
            else
            {
                databaseUser.Stats.OpenedBoosterPacks += 1;
            }

            _blockingPriorityQueue.TryEnqueue(new GiveBoosterPackMessage
            {
                DiscordUserId = discordId.Value,
                Cards = cards,
            });

            return Ok(cards);
        }

        /// <summary>
        /// Activates or deactivates card.
        /// </summary>
        /// <param name="wid">The card identifier.</param>
        [HttpPut("deck/toggle/card/{wid}"), Authorize(Policy = AuthorizePolicies.Player)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ToggleCardStatusAsync(ulong wid)
        {
            var discordId = _userContext.DiscordId;

            if (!discordId.HasValue)
            {
                return ShindenForbidden(Strings.ClaimNotFound);
            }

            var databaseUser = await _userRepository.GetCachedFullUserAsync(discordId.Value);

            if (databaseUser == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var card = databaseUser.GameDeck.Cards.FirstOrDefault(x => x.Id == wid);

            if (card == null)
            {
                return ShindenNotFound(Strings.CardNotFound);
            }

            if (card.InCage)
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