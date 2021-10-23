using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sanakan.Api.Models;
using Sanakan.Configuration;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Models;
using Sanakan.Extensions;
using Sanakan.Web.Configuration;
using Sanakan.Web.Models;
using static Sanakan.Web.ResponseExtensions;

namespace Sanakan.Web.Controllers
{
    [ApiController, Authorize(Policy = AuthorizePolicies.Site)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RichMessageController : ControllerBase
    {
        private readonly IOptionsMonitor<SanakanConfiguration> _config;
        private readonly IDiscordSocketClientAccessor _client;

        public RichMessageController(
            IDiscordSocketClientAccessor client,
            IOptionsMonitor<SanakanConfiguration> config)
        {
            _client = client;
            _config = config;
        }

        /// <summary>
        /// Deletes rich message
        /// </summary>
        /// <remarks>
        /// Do usunięcia wystarczy podać id poprzednio wysłanej wiadomości.
        /// </remarks>
        /// <param name="id">id wiadomości</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRichMessageAsync(ulong id)
        {
            var config = _config.CurrentValue;

            _ = Task.Run(async () =>
            {
                foreach (var rmc in config.RMConfig)
                {
                    var guild = _client.Client.GetGuild(rmc.GuildId);
                    
                    if (guild == null)
                    {
                        continue;
                    }

                    var channel = guild.GetTextChannel(rmc.ChannelId);
                    
                    if (channel == null)
                    {
                        continue;
                    }

                    var msg = await channel.GetMessageAsync(id);

                    if (msg == null)
                    {
                        continue;
                    }

                    await msg.DeleteAsync();
                    break;
                }
            });

            return ShindenOk("Message deleted!");
        }

        /// <summary>
        /// Modyfikuje wiadomość typu RichMessage
        /// </summary>
        /// <remarks>
        /// Do modyfikacji wiadomości należy podać wszystkie dane od nowa.
        /// Jeśli chcemy aby link z pola Url zadziałał to należy również sprecyzować tytuł wiadomości.
        /// </remarks>
        /// <param name="id">id wiadomości</param>
        /// <param name="message">wiadomość</param>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BodyPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> ModifyeRichMessageAsync(
            ulong id, [FromBody, Required]RichMessage message)
        {
            var config = _config.CurrentValue;

            _ = Task.Run(async () =>
            {
                foreach (var rmc in config.RMConfig)
                {
                    var guild = _client.Client.GetGuild(rmc.GuildId);

                    if (guild == null)
                    {
                        continue;
                    }
                    
                    var channel = guild.GetTextChannel(rmc.ChannelId);

                    if (channel == null)
                    {
                        continue;
                    }

                    var msg = await channel.GetMessageAsync(id);

                    if (msg == null)
                    {
                        continue;
                    }

                        await ((IUserMessage)msg).ModifyAsync(x => x.Embed = message.ToEmbed());
                    break;
                }
            });

            return Ok("Message modified!");
        }

        /// <summary>
        /// Sends rich message.
        /// </summary>
        /// <remarks>
        /// Do utworzenia wiadomości wystarczy ustawić jej typ oraz, podać opis.
        /// Jeśli chcemy aby link z pola Url zadziałał to należy również sprecyzować tytuł wiadomości.
        /// </remarks>
        /// <param name="message">wiadomość</param>
        /// <param name="mention">czy oznanczyć zainteresowanych</param>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostRichMessageAsync(
            [FromBody, Required]RichMessage message, [FromQuery]bool? mention)
        {
            var config = _config.CurrentValue;

            if (!mention.HasValue)
            {
                mention = false;
            }

            var msgList = new List<ulong>();
            var rmcs = config.RMConfig.Where(x => x.Type == message.MessageType);

            foreach (var rmc in rmcs)
            {
                if (rmc.Type == RichMessageType.UserNotify)
                {
                    var user = _client.Client.GetUser(rmc.ChannelId);
                    if (user == null)
                    {
                        continue;
                    }

                    var pwCh = await user.GetOrCreateDMChannelAsync();
                    var pwm = await pwCh.SendMessageAsync("", embed: message.ToEmbed());

                    msgList.Add(pwm.Id);
                    continue;
                }

                var guild = _client.Client.GetGuild(rmc.GuildId);
                
                if (guild == null)
                {
                    continue;
                }

                var channel = guild.GetTextChannel(rmc.ChannelId);

                if (channel == null)
                {
                    continue;
                }

                string mentionContent = "";
                if (mention.Value)
                {
                    var role = guild.GetRole(rmc.RoleId);
                    if (role != null)
                    {
                        mentionContent = role.Mention;
                    }
                }

                var msg = await channel.SendMessageAsync(mentionContent, embed: message.ToEmbed());
                if (msg != null) msgList.Add(msg.Id);
            }

            if (msgList.Count == 0)
            {
                return ShindenBadRequest("Message not send!");
            }

            if (msgList.Count > 1)
            {
                return ShindenRichOk("Message sended!", msgList.ToArray());
            }

            return ShindenRichOk("Message sended!", msgList.First());
        }

        /// <summary>
        /// Zwraca przykładową wiadomość typu RichMessage
        /// </summary>
        /// <returns>wiadomość typu RichMessage</returns>
        [HttpGet]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult GetExampleMsg()
        {
            var result = new RichMessage().Example();

            return Ok(result);
        }
    }
}