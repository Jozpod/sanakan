using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sanakan.Api.Models;
using Sanakan.Extensions;

namespace Sanakan.Web.Controllers
{
    [ApiController, Authorize(Policy = AuthorizePolicies.Site)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RichMessageController : ControllerBase
    {
        private readonly IConfig _config;
        private readonly DiscordSocketClient _client;

        public RichMessageController(
            DiscordSocketClient client,
            IConfig config)
        {
            _client = client;
            _config = config;
        }

        /// <summary>
        /// Kasuje wiadomość typu RichMessage
        /// </summary>
        /// <remarks>
        /// Do usunięcia wystarczy podać id poprzednio wysłanej wiadomości.
        /// </remarks>
        /// <param name="id">id wiadomości</param>
        /// <response code="404">Message not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete("{id}")]
        public async Task DeleteRichMessageAsync(ulong id)
        {
            var config = _config.Get();

            _ = Task.Run(async () =>
            {
                foreach (var rmc in config.RMConfig)
                {
                    var guild = _client.GetGuild(rmc.GuildId);
                    if (guild == null) continue;

                    var channel = guild.GetTextChannel(rmc.ChannelId);
                    if (channel == null) continue;

                    var msg = await channel.GetMessageAsync(id);
                    if (msg == null) continue;

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
        /// <response code="404">Message not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpPut("{id}")]
        public async Task ModifyeRichMessageAsync(ulong id, [FromBody, Required]RichMessage message)
        {
            var config = _config.Get();

            _ = Task.Run(async () =>
            {
                foreach (var rmc in config.RMConfig)
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
        /// Wysyła wiadomość typu RichMessage
        /// </summary>
        /// <remarks>
        /// Do utworzenia wiadomości wystarczy ustawić jej typ oraz, podać opis.
        /// Jeśli chcemy aby link z pola Url zadziałał to należy również sprecyzować tytuł wiadomości.
        /// </remarks>
        /// <param name="message">wiadomość</param>
        /// <param name="mention">czy oznanczyć zainteresowanych</param>
        /// <response code="500">Internal Server Error</response>
        [HttpPost]
        public async Task PostRichMessageAsync(
            [FromBody, Required]Models.RichMessage message, [FromQuery]bool? mention)
        {
            var config = _config.Get();
            if (!mention.HasValue) mention = false;

            var msgList = new List<ulong>();
            var rmcs = config.RMConfig.Where(x => x.Type == message.MessageType);

            foreach (var rmc in rmcs)
            {
                if (rmc.Type == RichMessageType.UserNotify)
                {
                    var user = _client.GetUser(rmc.ChannelId);
                    if (user == null) continue;

                    var pwCh = await user.GetOrCreateDMChannelAsync();
                    var pwm = await pwCh.SendMessageAsync("", embed: message.ToEmbed());

                    msgList.Add(pwm.Id);
                    continue;
                }

                var guild = _client.GetGuild(rmc.GuildId);
                if (guild == null) continue;

                var channel = guild.GetTextChannel(rmc.ChannelId);
                if (channel == null) continue;

                string mentionContent = "";
                if (mention.Value)
                {
                    var role = guild.GetRole(rmc.RoleId);
                    if (role != null) mentionContent = role.Mention;
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
                return ShindenRichOk("Message sended!", msgList);
            }

            return ShindenRichOk("Message sended!", msgList.First());
        }

        /// <summary>
        /// Zwraca przykładową wiadomość typu RichMessage
        /// </summary>
        /// <returns>wiadomość typu RichMessage</returns>
        [HttpGet]
        public IActionResult GetExampleMsg()
        {
            var result = new Models.RichMessage().Example();

            return Ok(result);
        }
    }
}