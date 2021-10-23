using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sanakan.DiscordBot;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;

        public DebugController(
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            ILogger<DebugController> logger)
        {
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Zabija bota
        /// </summary>
        [HttpPost("kill"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> RestartBotAsync()
        {
            await _discordSocketClientAccessor.Client.LogoutAsync();
            _logger.LogDebug("Kill app from web.");
            return Ok();
        }

        /// <summary>
        /// Aktualizuje bota
        /// </summary>
        [HttpPost("update"), Authorize(Policy = AuthorizePolicies.Site)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateBotAsync()
        {
            await _discordSocketClientAccessor.Client.LogoutAsync();
            //System.IO.File.Create("./updateNow");
            _logger.LogDebug("Update app from web.");
            return Ok();
        }
    }
}