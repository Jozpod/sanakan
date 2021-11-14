using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Config;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Configuration;
using Sanakan.Web.Resources;
using Shinden;
using static Sanakan.Web.ResponseExtensions;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;
        private readonly IUserRepository _userRepository;
        private readonly IBlockingPriorityQueue _blockingPriorityQueue;
        private readonly ITransferAnalyticsRepository _transferAnalyticsRepository;
        private readonly IShindenClient _shindenClient;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;
        private readonly IUserContext _userContext;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly IRequestBodyReader _requestBodyReader;

        public UserController(
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<SanakanConfiguration> config,
            IUserRepository userRepository,
            ITransferAnalyticsRepository transferAnalyticsRepository,
            IShindenClient shClient,
            ILogger<UserController> logger,
            ISystemClock systemClock,
            ICacheManager cacheManager,
            IUserContext userContext,
            IJwtBuilder jwtBuilder,
            IRequestBodyReader requestBodyReader)
        {
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _config = config;
            _userRepository = userRepository;
            _transferAnalyticsRepository = transferAnalyticsRepository;
            _logger = logger;
            _shindenClient = shClient;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
            _userContext = userContext;
            _jwtBuilder = jwtBuilder;
            _requestBodyReader = requestBodyReader;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="discordUserId">The user identifier in Discord.</param>
        [HttpGet("discord/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByDiscordIdAsync(ulong discordUserId)
        {
            var result = await _userRepository.GetCachedFullUserAsync(discordUserId);
            return Ok(result);
        }

        /// <summary>
        /// Find the user in Shinden.
        /// </summary>
        /// <param name="name">The name of the user</param>
        [HttpPost("find")]
        [ProducesResponseType(typeof(IEnumerable<UserSearchResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserIdByNameAsync([FromBody, Required]string name)
        {
            var searchUserResult = await _shindenClient.SearchUserAsync(name);

            if (searchUserResult.Value == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            return Ok(searchUserResult);
        }

        /// <summary>
        /// Gets the username from Shinden.
        /// </summary>
        /// <param name="shindenUserId">The Shinden user identifier.</param>
        [HttpGet("shinden/{shindenUserId}/username")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShindenUsernameByShindenId(ulong shindenUserId)
        {
            var userResult = await _shindenClient.GetUserInfoAsync(shindenUserId);

            if (userResult.Value == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var username = userResult.Value.Name;

            return Ok(username);
        }

        /// <summary>
        /// Gets the user with token.
        /// </summary>
        /// <param name="id">The Shinden user identifier.</param>
        [HttpGet("shinden/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByShindenIdAsync(ulong id)
        {
            var user = await _userRepository.GetCachedFullUserByShindenIdAsync(id);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            TokenData tokenData = null;
            
            if (_userContext.HasWebpageClaim())
            {
                tokenData = _jwtBuilder.Build(_config.CurrentValue.SanakanApi.UserWithTokenExpiry);
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
        /// Pobieranie użytkownika bota z zmniejszoną ilością danych
        /// </summary>
        /// <param name="id">The Shinden user identifier.</param>
        [HttpGet("shinden/simple/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByShindenIdSimpleAsync(ulong id)
        {
            var user = await _userRepository.GetByShindenIdAsync(id, new UserQueryOptions
            {
                IncludeGameDeck = true
            });

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            TokenData tokenData = null;
            
            if (_userContext.HasWebpageClaim())
            {
                tokenData = _jwtBuilder.Build(_config.CurrentValue.SanakanApi.UserWithTokenExpiry);
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
        /// Zmienia użytkownikowi shindena nick
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        /// <param name="nickname">ksywka użytkownika</param>
        /// <response code="404">User not found</response>
        [HttpPost("shinden/{id}/nickname"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeNicknameShindenUserAsync(
            ulong id,
            [FromBody, Required]string nickname)
        {
            var user = await _userRepository.GetByShindenIdAsync(id);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var client = _discordSocketClientAccessor.Client;

            if(client == null)
            {
                return ShindenForbidden("Could not access discord client");
            }

            var guild = client.GetGuild(Constants.ShindenDiscordGuildId);

            if (guild == null)
            {
                return ShindenNotFound("Guild not found!");
            }

            var userOnGuild = guild.GetUser(user.Id);

            if (userOnGuild == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            await userOnGuild.ModifyAsync(x => x.Nickname = nickname);

            return ShindenOk("User nickname changed!");
        }

        /// <summary>
        /// Connects the shinden user with the discord user.
        /// </summary>
        [HttpPut("register"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterUserAsync([FromBody, Required]UserRegistration model)
        {
            if (model == null)
            {
                var requestBody = await _requestBodyReader.GetStringAsync();

                _logger.LogDebug(requestBody);

                return ShindenInternalServerError("Model is Invalid!");
            }

            var client = _discordSocketClientAccessor.Client;

            if (client == null)
            {
                return ShindenForbidden("Could not access discord client");
            }

            var discordUser = client.GetUser(model.DiscordUserId);
            
            if (discordUser == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var botUser = await _userRepository.GetByDiscordIdAsync(model.DiscordUserId);

            if (botUser != null || botUser.ShindenId.HasValue)
            {
                return ShindenNotFound("User already connected!");
            }

            var searchUserResult = await _shindenClient.SearchUserAsync(model.Username);

            if (searchUserResult.Value == null)
            {
                return ShindenForbidden("Can't connect to shinden!");
            }

            var firstMatch = searchUserResult.Value.First();

            var userResult = await _shindenClient.GetUserInfoAsync(firstMatch.Id);

            if (userResult.Value == null)
            {
                return ShindenForbidden("Can't connect to shinden!");
            }

            var shindenUser = userResult.Value;

            if (shindenUser.Id.Value != model.ForumUserId)
            {
                return ShindenInternalServerError("Something went wrong!");
            }

            if (await _userRepository.ExistsByShindenIdAsync(shindenUser.Id.Value))
            {
                var oldUsers = await _userRepository
                    .GetByShindenIdExcludeDiscordIdAsync(shindenUser.Id.Value, model.DiscordUserId);

                if (oldUsers.Any())
                {
                    var rmcs = _config
                        .CurrentValue
                        .RMConfig
                        .Where(x => x.Type == RichMessageType.AdminNotify);
                    foreach (var rmc in rmcs)
                    {
                        var guild = client.GetGuild(rmc.GuildId);
                        if (guild == null)
                        {
                            continue;
                        }

                        var channel = guild.GetTextChannel(rmc.ChannelId);
                        if (channel == null)
                        {
                            continue;
                        }

                        var users = string.Join(",", oldUsers.Select(x => x.Id));

                        var content = ($"Potencjalne multikonto:\nDID: {model.DiscordUserId}\nSID: {shindenUser.Id.Value}\n"
                            + $"SN: {shindenUser.Name}\n\noDID: {users}").ElipseTrimToLength(2000)
                            .ToEmbedMessage(EMType.Error).Build();

                        await channel.SendMessageAsync("", embed: content);
                    }
                }

                return ShindenUnauthorized("This account is already linked!");
            }

            _blockingPriorityQueue.TryEnqueue(new ConnectUserMessage
            {
                DiscordUserId = model.DiscordUserId,
                ShindenUserId = shindenUser.Id.Value,
            });

            return ShindenOk("User connected!");
        }

        /// <summary>
        /// Changes the amount of TC points for given discord user.
        /// </summary>
        /// <param name="discordUserId">The user identifier in Discord.</param>
        /// <param name="amount">The number of TC</param>
        [HttpPut("discord/{discordUserId}/tc"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModifyPointsTCDiscordAsync(
            ulong discordUserId, [FromBody, Required]ulong amount)
        {
            var user = await _userRepository.GetByDiscordIdAsync(discordUserId);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            _blockingPriorityQueue.TryEnqueue(new TransferTCMessage
            {
                DiscordUserId = discordUserId,
                Amount = amount
            });

            return ShindenOk("TC added!");
        }

        /// <summary>
        /// Changes TC points for given Shinden user.
        /// </summary>
        /// <param name="shindenUserId">The user identifier in Shinden.</param>
        /// <param name="value">liczba TC</param>
        [HttpPut("shinden/{shindenUserId}/tc"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> ModifyPointsTCAsync(
            ulong shindenUserId, [FromBody, Required]ulong amount)
        {
            var user = await _userRepository.GetByShindenIdAsync(shindenUserId);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            _blockingPriorityQueue.TryEnqueue(new TransferTCMessage
            {
                ShindenUserId = shindenUserId,
                Amount = amount
            });

            return ShindenOk("TC added!");
        }
    }
}