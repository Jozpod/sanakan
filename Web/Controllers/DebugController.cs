using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sanakan.Common;
using Sanakan.DiscordBot;
using System.Threading.Tasks;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DebugController : ControllerBase
    {
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public DebugController(
            IDiscordClientAccessor discordClientAccessor,
            IFileSystem fileSystem,
            ILogger<DebugController> logger)
        {
            _discordClientAccessor = discordClientAccessor;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        /// <summary>
        /// Logouts discord client bot.
        /// </summary>
        [HttpPost("kill"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> RestartBotAsync()
        {
            await _discordClientAccessor.LogoutAsync();
            _logger.LogDebug("Kill app from web.");
            return Ok();
        }

        /// <summary>
        /// Updates bot.
        /// </summary>
        [HttpPost("update"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateBotAsync()
        {
            await _discordClientAccessor.LogoutAsync();
            _fileSystem.Create(Placeholders.UpdateNow);
            _logger.LogDebug("Update app from web.");
            return Ok();
        }
    }
}