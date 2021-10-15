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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Config;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.Extensions;
using Sanakan.Services.Executor;
using Sanakan.ShindenApi;
using Shinden;
using Shinden.Models;
using static Sanakan.Web.ResponseExtensions;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IRepository _repository;
        private readonly IExecutor _executor;
        private readonly IShindenClient _shClient;
        private readonly DiscordSocketClient _client;
        private readonly ISystemClock _systemClock;
        private readonly ICacheManager _cacheManager;

        public UserController(
            DiscordSocketClient client,
            IUserRepository userRepository,
            IRepository repository,
            IShindenClient shClient,
            ILogger<UserController> logger,
            IExecutor executor,
            ISystemClock systemClock,
            ICacheManager cacheManager)
        {
            _client = client;
            _userRepository = userRepository;
            _repository = repository;
            _logger = logger;
            _executor = executor;
            _shClient = shClient;
            _systemClock = systemClock;
            _cacheManager = cacheManager;
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
            var response = await _shClient.Search.UserAsync(name);

            if (!response.IsSuccessStatusCode())
            {
                return ShindenNotFound("User not found!");
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
            var response = await _shClient.User.GetAsync(id);

            if (!response.IsSuccessStatusCode())
            {
                return ShindenNotFound("User not found!");
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
                return ShindenNotFound("User not found!");
            }

            TokenData tokenData = null;
            var currUser = ControllerContext.HttpContext.User;

            if (currUser.HasClaim(x => x.Type == ClaimTypes.Webpage))
            {
                tokenData = JwtBuilder.BuildUserToken(_config, user);
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
        [HttpGet("shinden/simple/{id}"), Authorize(Policy = "Site")]
        public async Task<UserWithToken> GetUserByShindenIdSimpleAsync(ulong id)
        {
            using (var db = new Database.UserContext(_config))
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
                    await "User not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                    return null;
                }

                TokenData tokenData = null;
                var currUser = ControllerContext.HttpContext.User;
                if (currUser.HasClaim(x => x.Type == ClaimTypes.Webpage))
                {
                    tokenData = JwtBuilder.BuildUserToken(_config, user);
                }

                return new UserWithToken()
                {
                    Expire = tokenData?.Expire,
                    Token = tokenData?.Token,
                    User = user,
                };
            }
        }

        /// <summary>
        /// Zmienia użytkownikowi shindena nick
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        /// <param name="nickname">ksywka użytkownika</param>
        /// <response code="404">User not found</response>
        [HttpPost("shinden/{id}/nickname"), Authorize(Policy = "Site")]
        public async Task ChangeNicknameShindenUserAsync(ulong id, [FromBody, Required]string nickname)
        {
            using (var db = new Database.UserContext(_config))
            {
                var user = await db.Users.AsQueryable().AsSplitQuery().Where(x => x.Shinden == id).AsNoTracking().FirstOrDefaultAsync();
                if (user == null)
                {
                    await "User not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                    return;
                }

                var guild = _client.GetGuild(245931283031523330);
                if (guild == null)
                {
                    await "Guild not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                    return;
                }

                var userOnGuild = guild.GetUser(user.Id);
                if (userOnGuild == null)
                {
                    await "User not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                    return;
                }

                await userOnGuild.ModifyAsync(x => x.Nickname = nickname);
            }

            await "User nickname changed!".ToResponse(200).ExecuteResultAsync(ControllerContext);
        }

        /// <summary>
        /// Pełne łączenie użytkownika
        /// </summary>
        /// <param name="id">relacja</param>
        /// <response code="403">Can't connect to shinden!</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Model is invalid!</response>
        [HttpPut("register"), Authorize(Policy = "Site")]
        public async Task RegisterUserAsync([FromBody, Required]UserRegistration id)
        {
            if (id == null)
            {
                var body = new System.IO.StreamReader(ControllerContext.HttpContext.Request.Body);
                body.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                var requestBody = body.ReadToEnd();

                _logger.Log(requestBody);

                await "Model is Invalid!".ToResponse(500).ExecuteResultAsync(ControllerContext);
                return;
            }

            var user = _client.GetUser(id.DiscordUserId);
            if (user == null)
            {
                await "User not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                return;
            }

            using (var db = new Database.UserContext(_config))
            {
                var botUser = db.Users.FirstOrDefault(x => x.Id == id.DiscordUserId);
                if (botUser != null)
                {
                    if (botUser.Shinden != 0)
                    {
                        await "User already connected!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                        return;
                    }
                }

                var response = await _shClient.Search.UserAsync(id.Username);
                if (!response.IsSuccessStatusCode())
                {
                    await "Can't connect to shinden!".ToResponse(403).ExecuteResultAsync(ControllerContext);
                    return;
                }

                var sUser = (await _shClient.User.GetAsync(response.Body.First())).Body;
                if (sUser.ForumId.Value != id.ForumUserId)
                {
                    await "Something went wrong!".ToResponse(500).ExecuteResultAsync(ControllerContext);
                    return;
                }

                if (db.Users.Any(x => x.Shinden == sUser.Id))
                {
                    await "This account is already linked!".ToResponse(401).ExecuteResultAsync(ControllerContext);
                    var oldUsers = await db.Users.AsQueryable().Where(x => x.Shinden == sUser.Id && x.Id != id.DiscordUserId).ToListAsync();

                    if (oldUsers.Count > 0)
                    {
                        var rmcs = _config.Get().RMConfig.Where(x => x.Type == RichMessageType.AdminNotify);
                        foreach (var rmc in rmcs)
                        {
                            var guild = _client.GetGuild(rmc.GuildId);
                            if (guild == null) continue;

                            var channel = guild.GetTextChannel(rmc.ChannelId);
                            if (channel == null) continue;

                            await channel.SendMessageAsync("", embed: ($"Potencjalne multikonto:\nDID: {id.DiscordUserId}\nSID: {sUser.Id}\n"
                                + $"SN: {sUser.Name}\n\noDID: {string.Join(",", oldUsers.Select(x => x.Id))}").TrimToLength(2000).ToEmbedMessage(EMType.Error).Build());
                        }
                    }
                    return;
                }

                var exe = new Executable($"api-register u{id.DiscordUserId}", new Task<Task>(async () =>
                {
                    using (var dbs = new Database.UserContext(_config))
                    {
                        botUser = await dbs.GetUserOrCreateAsync(id.DiscordUserId);
                        botUser.Shinden = sUser.Id;

                        await dbs.SaveChangesAsync();

                        _cacheManager.ExpireTag(new string[] { $"user-{user.Id}", "users" });
                    }
                }), Priority.High);

                await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
                await "User connected!".ToResponse(200).ExecuteResultAsync(ControllerContext);
            }
        }

        /// <summary>
        /// Zmiana ilości punktów TC użytkownika
        /// </summary>
        /// <param name="id">id użytkownika discorda</param>
        /// <param name="value">liczba TC</param>
        /// <response code="404">User not found</response>
        [HttpPut("discord/{id}/tc"), Authorize(Policy = AuthorizePolicies.Site)]
        public async Task ModifyPointsTCDiscordAsync(ulong id, [FromBody, Required]long value)
        {
            using (var db = new Database.UserContext(_config))
            {
                var user = db.Users.FirstOrDefault(x => x.Id == id);
                if (user == null)
                {
                    await "User not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                    return;
                }

                var exe = new Executable($"api-tc u{id} ({value})", new Task<Task>(async () =>
                {
                    using (var dbc = new Database.AnalyticsContext(_config))
                    {
                        dbc.TransferData.Add(new Database.Models.Analytics.TransferAnalytics()
                        {
                            Value = value,
                            DiscordId = user.Id,
                            Date = _systemClock.UtcNow,
                            ShindenId = user.Shinden,
                            Source = TransferSource.ByDiscordId,
                        });

                        await dbc.SaveChangesAsync();
                    }

                    using (var dbs = new Database.UserContext(_config))
                    {
                        user = dbs.GetUserOrCreateAsync(id).Result;
                        user.TcCnt += value;

                        dbs.SaveChanges();

                        _cacheManager.ExpireTag(new string[] { $"user-{user.Id}", "users" });
                    }
                }), Priority.High);

                await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
                await "TC added!".ToResponse(200).ExecuteResultAsync(ControllerContext);
            }
        }

        /// <summary>
        /// Zmiana ilości punktów TC użytkownika
        /// </summary>
        /// <param name="id">id użytkownika shindena</param>
        /// <param name="value">liczba TC</param>
        /// <response code="404">User not found</response>
        [HttpPut("shinden/{id}/tc"), Authorize(Policy = "Site")]
        public async Task ModifyPointsTCAsync(ulong id, [FromBody, Required]long value)
        {
            using (var db = new Database.UserContext(_config))
            {
                var user = db.Users.FirstOrDefault(x => x.Shinden == id);
                if (user == null)
                {
                    await "User not found!".ToResponse(404).ExecuteResultAsync(ControllerContext);
                    return;
                }

                var exe = new Executable($"api-tc su{id} ({value})", new Task<Task>(async () =>
                {
                    using (var dbs = new Database.UserContext(_config))
                    {
                        user = dbs.Users.FirstOrDefault(x => x.Shinden == id);
                        user.TcCnt += value;

                        await dbs.SaveChangesAsync();

                        _cacheManager.ExpireTag(new string[] { $"user-{user.Id}", "users" });
                    }

                    using (var dbc = new Database.AnalyticsContext(_config))
                    {
                        dbc.TransferData.Add(new Database.Models.Analytics.TransferAnalytics()
                        {
                            Value = value,
                            DiscordId = user.Id,
                            Date = _systemClock.UtcNow,
                            ShindenId = user.Shinden,
                            Source = Database.Models.Analytics.TransferSource.ByShindenId,
                        });

                        await dbc.SaveChangesAsync();
                    }
                }), Priority.High);

                await _executor.TryAdd(exe, TimeSpan.FromSeconds(1));
                await "TC added!".ToResponse(200).ExecuteResultAsync(ControllerContext);
            }
        }
    }
}