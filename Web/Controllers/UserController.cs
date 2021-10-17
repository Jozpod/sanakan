using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories.Abstractions;
using Discord.WebSocket;
using DiscordBot.Services.Executor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Config;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using Sanakan.ShindenApi;
using Sanakan.Web.Configuration;
using Sanakan.Web.Resources;
using Shinden;
using Shinden.Models;
using static Sanakan.Web.ResponseExtensions;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly SanakanConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IRepository _repository;
        private readonly IExecutor _executor;
        private readonly IShindenClient _shClient;
        private readonly DiscordSocketClient _client;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;
        private readonly IUserContext _userContext;
        private readonly IJwtBuilder _jwtBuilder;

        public UserController(
            DiscordSocketClient client,
            IOptions<SanakanConfiguration> options,
            IUserRepository userRepository,
            IRepository repository,
            IShindenClient shClient,
            ILogger<UserController> logger,
            IExecutor executor,
            ISystemClock systemClock,
            ICacheManager cacheManager,
            IUserContext userContext,
            IJwtBuilder jwtBuilder)
        {
            _client = client;
            _userRepository = userRepository;
            _repository = repository;
            _logger = logger;
            _executor = executor;
            _shClient = shClient;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
            _userContext = userContext;
            _jwtBuilder = jwtBuilder;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="id">The user identifier in Discord.</param>
        [HttpGet("discord/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> GetUserByDiscordIdAsync(ulong id)
        {
            var result = await _userRepository.GetCachedFullUserAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Find the user in Shinden.
        /// </summary>
        /// <param name="name">The name of the user</param>
        [HttpPost("find")]
        [ProducesResponseType(typeof(IEnumerable<IUserSearch>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserIdByNameAsync([FromBody, Required]string name)
        {
            var response = await _shClient.UserAsync(name);

            if (!response.IsSuccessStatusCode())
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            return Ok(response.Body);
        }

        /// <summary>
        /// Gets the username from Shinden.
        /// </summary>
        /// <param name="id">The Shinden user identifier.</param>
        [HttpGet("shinden/{id}/username")]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShindenUsernameByShindenId(ulong id)
        {
            var response = await _shClient.GetAsync(id);

            if (!response.IsSuccessStatusCode())
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var username = response.Body.Name;

            return Ok(username);
        }

        /// <summary>
        /// Gets the user
        /// </summary>
        /// <param name="id">The Shinden user identifier.</param>
        [HttpGet("shinden/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
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
                tokenData = _jwtBuilder.Build();
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
        /// <param name="id">id użytkownika shindena</param>
        /// <returns>użytkownik bota</returns>
        [HttpGet("shinden/simple/{id}"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserByShindenIdSimpleAsync(ulong id)
        {
            var user = db.Users
                .AsQueryable()
                .AsSplitQuery()
                .Where(x => x.Shinden == id)
                .Include(x => x.GameDeck)
                .AsNoTracking()
                .FirstOrDefault();

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            TokenData tokenData = null;
            
            if (_userContext.HasWebpageClaim())
            {
                tokenData = _jwtBuilder.Build(_config, user);
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
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeNicknameShindenUserAsync(ulong id, [FromBody, Required]string nickname)
        {
            var user = await db.Users
                     .AsQueryable()
                     .AsSplitQuery()
                     .Where(x => x.Shinden == id)
                     .AsNoTracking()
                     .FirstOrDefaultAsync();

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var guild = _client.GetGuild(Constants.ShindenDiscordGuildId);

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
        /// Pełne łączenie użytkownika
        /// </summary>
        /// <param name="id">relacja</param>
        [HttpPut("register"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(UserWithToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterUserAsync([FromBody, Required]UserRegistration id)
        {
            if (id == null)
            {
                var body = new System.IO.StreamReader(ControllerContext.HttpContext.Request.Body);
                body.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                var requestBody = body.ReadToEnd();

                _logger.LogDebug(requestBody);

                return ShindenInternalServerError("Model is Invalid!");
            }

            var user = _client.GetUser(id.DiscordUserId);
            
            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var botUser = db.Users.FirstOrDefault(x => x.Id == id.DiscordUserId);
            if (botUser != null || botUser.Shinden != 0)
            {
                return ShindenNotFound("User already connected!");
            }

            var response = await _shClient.Search.UserAsync(id.Username);
            if (!response.IsSuccessStatusCode())
            {
                return ShindenForbidden("Can't connect to shinden!");
            }

            var sUser = (await _shClient.User.GetAsync(response.Body.First())).Body;
            if (sUser.ForumId.Value != id.ForumUserId)
            {
                return ShindenInternalServerError("Something went wrong!");
            }

            if (db.Users.Any(x => x.Shinden == sUser.Id))
            {
                var oldUsers = await db.Users.AsQueryable()
                    .Where(x => x.Shinden == sUser.Id
                        && x.Id != id.DiscordUserId)
                    .ToListAsync();

                if (oldUsers.Any())
                {
                    var rmcs = _config.RMConfig
                        .Where(x => x.Type == RichMessageType.AdminNotify);
                    foreach (var rmc in rmcs)
                    {
                        var guild = _client.GetGuild(rmc.GuildId);
                        if (guild == null)
                        {
                            continue;
                        }

                        var channel = guild.GetTextChannel(rmc.ChannelId);
                        if (channel == null)
                        {
                            continue;
                        }

                        var content = ($"Potencjalne multikonto:\nDID: {id.DiscordUserId}\nSID: {sUser.Id}\n"
                            + $"SN: {sUser.Name}\n\noDID: {string.Join(",", oldUsers.Select(x => x.Id))}").TrimToLength(2000)
                            .ToEmbedMessage(EMType.Error).Build();

                        await channel.SendMessageAsync("", embed: content);
                    }
                }

                return ShindenUnauthorized("This account is already linked!");
            }
            var exe = new Executable($"api-register u{id.DiscordUserId}", new Task<Task>(async () =>
            {
                botUser = await dbs.GetUserOrCreateAsync(id.DiscordUserId);
                botUser.Shinden = sUser.Id;

                await dbs.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"user-{user.Id}", "users" });
            }), Priority.High);

            await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
            return ShindenOk("User connected!");
        }

        /// <summary>
        /// Zmiana ilości punktów TC użytkownika
        /// </summary>
        /// <param name="id">id użytkownika discorda</param>
        /// <param name="value">liczba TC</param>
        /// <response code="404">User not found</response>
        [HttpPut("discord/{id}/tc"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModifyPointsTCDiscordAsync(ulong id, [FromBody, Required]long value)
        {
            var user = await _userRepository.GetByIdAsync();
            //db.Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var exe = new Executable($"api-tc u{id} ({value})", new Task<Task>(async () =>
            {
                var record = new TransferAnalytics()
                {
                    Value = value,
                    DiscordId = user.Id,
                    Date = _systemClock.UtcNow,
                    ShindenId = user.Shinden,
                    Source = TransferSource.ByDiscordId,
                };

                await _repository.AddTransferAnalyticsAsync(record);

                user = await _repository.GetUserOrCreateAsync(id);
                user.TcCnt += value;

                await _repository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"user-{user.Id}", "users" });
            }), Priority.High);

            await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
            return ShindenOk("TC added!");
        }

        /// <summary>
        /// Zmiana ilości punktów TC użytkownika
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        /// <param name="value">liczba TC</param>
        [HttpPut("shinden/{id}/tc"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> ModifyPointsTCAsync(ulong id, [FromBody, Required]long value)
        {
            var user = await _userRepository.GetByShindenIdAsync(id);

            if (user == null)
            {
                return ShindenNotFound(Strings.UserNotFound);
            }

            var exe = new Executable($"api-tc su{id} ({value})", new Task<Task>(async () =>
            {
                user = await _userRepository.GetByShindenIdAsync(id);
                user.TcCnt += value;

                await _userRepository.SaveChangesAsync();

                _cacheManager.ExpireTag(new string[] { $"user-{user.Id}", "users" });

                var record = new TransferAnalytics()
                {
                    Value = value,
                    DiscordId = user.Id,
                    Date = _systemClock.UtcNow,
                    ShindenId = user.Shinden,
                    Source = TransferSource.ByShindenId,
                };

                await _repository.AddTransferAnalyticsAsync(record);
            }), Priority.High);

            await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
            return ShindenOk("TC added!");
        }
    }
}