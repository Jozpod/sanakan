using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sanakan.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;

        public DebugController(
            DiscordSocketClient client,
            ILogger<DebugController> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Zabija bota
        /// </summary>
        [HttpPost("kill"), Authorize(Policy = AuthorizePolicies.Site)]
        public async Task<IActionResult> RestartBotAsync()
        {
            await _client.LogoutAsync();
            _logger.LogDebug("Kill app from web.");
            return Ok();
        }

        /// <summary>
        /// Aktualizuje bota
        /// </summary>
        [HttpPost("update"), Authorize(Policy = "Site")]
        public async Task<IActionResult> UpdateBotAsync()
        {
            await _client.LogoutAsync();
            //System.IO.File.Create("./updateNow");
            _logger.LogDebug("Update app from web.");
            return Ok();
        }
    }
}