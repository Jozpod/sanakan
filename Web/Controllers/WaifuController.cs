﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Sanakan.Web.Resources;
using static Sanakan.Web.ResponseExtensions;
using Sanakan.ShindenApi.Utilities;
using Microsoft.Extensions.Options;
using Sanakan.TaskQueue.Messages;
using Sanakan.Game.Models;
using Sanakan.Common.Configuration;
using Sanakan.TaskQueue;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Common.Cache;
using System.Security.Claims;

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

            var tagList = new List<string>();
            var tags = user.GameDeck.Cards
                .Where(x => x.TagList != null)
                .Select(x => x.TagList.Select(c => c.Name));
            
            foreach (var tag in tags)
            {
                tagList.AddRange(tag);
            }

            var gameDeck = user.GameDeck;
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
                    {"PC", user.GameDeck.PVPCoins},
                    {"CT", user.GameDeck.CTCount},
                    {"AC", user.AcCount},
                    {"TC", user.TcCount},
                    {"SC", user.ScCount},
                };

            var waifuCard = user.GameDeck.Cards.Where(x => x.CharacterId == user.GameDeck
                .FavouriteWaifuId)
                .OrderBy(x => x.Rarity).ThenByDescending(x => x.Quality)
                .FirstOrDefault();

            var waifu = waifuCard == null ? null : new CardFinalView(waifuCard);

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
                BackgroundImageUrl = user.GameDeck.BackgroundImageUrl.ToString(),
                ForegroundImageUrl = user.GameDeck.ForegroundImageUrl.ToString(),
                Expeditions = user.GameDeck.Cards.Where(x => x.Expedition != ExpeditionCardType.None).ToExpeditionView(user.GameDeck.Karma),
                Waifu = waifu,
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

            TokenData tokenData = null;

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

            if(!enqueued)
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
                return new ObjectResult("The appropriate claim was not found")
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                };
            }

            var boosterPackName = "";
            var cards = new List<Card>();
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

            cards = await _waifuService.OpenBoosterPackAsync(null, pack);
            boosterPackName = pack.Name;

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