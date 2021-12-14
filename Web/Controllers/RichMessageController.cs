using Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Models;
using Sanakan.Web.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Sanakan.Web.ResponseExtensions;

namespace Sanakan.Web.Controllers
{
    [ApiController, Authorize(Policy = AuthorizePolicies.Site)]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RichMessageController : ControllerBase
    {
        private readonly ISystemClock _systemClock;
        private readonly IDiscordClientAccessor _client;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;

        public RichMessageController(
            ISystemClock systemClock,
            IDiscordClientAccessor client,
            IOptionsMonitor<SanakanConfiguration> config)
        {
            _systemClock = systemClock;
            _client = client;
            _config = config;
        }

        /// <summary>
        /// Deletes rich message
        /// </summary>
        [HttpDelete("{messageId}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRichMessageAsync(ulong messageId)
        {
            var config = _config.CurrentValue;

            var client = _client.Client;

            if (client == null)
            {
                return ShindenInternalServerError("Cannot access discord client.");
            }

            foreach (var richMessageConfig in config.RMConfig)
            {
                var guild = await client.GetGuildAsync(richMessageConfig.GuildId);

                if (guild == null)
                {
                    continue;
                }

                var channel = await guild.GetTextChannelAsync(richMessageConfig.ChannelId);

                if (channel == null)
                {
                    continue;
                }

                var message = await channel.GetMessageAsync(messageId);

                if (message == null)
                {
                    continue;
                }

                await message.DeleteAsync();
                break;
            }

            return ShindenOk("Message deleted!");
        }

        /// <summary>
        /// Modifies rich type message.
        /// </summary>
        /// <remarks>
        /// All fields must be supplied or message is generated from scratch.
        /// The title of message must be provided for URL field to work properly.
        /// </remarks>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="richMessage">The message</param>
        [HttpPut("{messageId}")]
        [ProducesResponseType(typeof(ShindenPayload), StatusCodes.Status200OK)]
        public async Task<IActionResult> ModifyRichMessageAsync(
            ulong messageId, [FromBody, Required]RichMessage richMessage)
        {
            var config = _config.CurrentValue;

            var client = _client.Client;

            if (client == null)
            {
                return ShindenInternalServerError("Cannot access discord client.");
            }

            foreach (var richMessageConfig in config.RMConfig)
            {
                var guild = await client.GetGuildAsync(richMessageConfig.GuildId);

                if (guild == null)
                {
                    continue;
                }

                var channel = await guild.GetTextChannelAsync(richMessageConfig.ChannelId);

                if (channel == null)
                {
                    continue;
                }

                var messgae = (IUserMessage)await channel.GetMessageAsync(messageId);

                if (messgae == null)
                {
                    continue;
                }

                await messgae.ModifyAsync(x => x.Embed = richMessage.ToEmbed());
                break;
            }

            return Ok("Message modified!");
        }

        /// <summary>
        /// Sends rich message.
        /// </summary>
        /// <remarks>
        /// The minimum amount of information required for new message is type and description.
        /// The title of message must be provided for URL field to work.
        /// </remarks>
        /// <param name="message">The message content.</param>
        /// <param name="mention">The mention option.</param>
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

            var client = _client.Client;

            if (client == null)
            {
                return ShindenInternalServerError("Cannot access discord client.");
            }

            var messageList = new List<ulong>();
            var richMessageConfigEntries = config.RMConfig.Where(x => x.Type == message.MessageType);

            foreach (var richMessageConfig in richMessageConfigEntries)
            {
                if (richMessageConfig.Type == RichMessageType.UserNotify)
                {
                    var user = await client.GetUserAsync(richMessageConfig.ChannelId);
                    if (user == null)
                    {
                        continue;
                    }

                    var dmChannel = await user.GetOrCreateDMChannelAsync();
                    var embed = message.ToEmbed();
                    var privateMessage = await dmChannel.SendMessageAsync(embed: embed);

                    messageList.Add(privateMessage.Id);
                    continue;
                }

                var guild = await client.GetGuildAsync(richMessageConfig.GuildId);

                if (guild == null)
                {
                    continue;
                }

                var channel = await guild.GetTextChannelAsync(richMessageConfig.ChannelId);

                if (channel == null)
                {
                    continue;
                }

                var mentionContent = "";

                if (mention.Value)
                {
                    var role = guild.GetRole(richMessageConfig.RoleId);

                    if (role != null)
                    {
                        mentionContent = role.Mention;
                    }
                }

                var discordMessage = await channel.SendMessageAsync(mentionContent, embed: message.ToEmbed());

                if (discordMessage != null)
                {
                    messageList.Add(discordMessage.Id);
                }
            }

            if (!messageList.Any())
            {
                return ShindenBadRequest("Message not send!");
            }

            if (messageList.Count > 1)
            {
                return ShindenRichOk("Message sended!", messageList.ToArray());
            }

            return ShindenRichOk("Message sended!", messageList.First());
        }

        /// <summary>
        /// Returns example rich type message.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public IActionResult GetExampleMessage()
        {
            var result = new RichMessage
            {
                Content = "sample content",
                Timestamp = _systemClock.UtcNow,
                Url = "https://gooogle.com",
                Title = "Max 256 characters",
                MessageType = RichMessageType.News,
                Description = "Max 2048 characters",
                ImageUrl = "http://cdn.shinden.eu/cdn1/avatars/225x350/85.jpg",
                ThumbnailUrl = "http://cdn.shinden.eu/cdn1/avatars/225x350/85.jpg",
                Author = new RichMessageAuthor
                {
                    Name = "Max 256 characters",
                    NameUrl = "https://gooogle.com",
                    ImageUrl = "http://cdn.shinden.eu/cdn1/avatars/225x350/85.jpg",
                },
                Footer = new RichMessageFooter
                {
                    Text = "Max 2048 characters",
                    ImageUrl = "http://cdn.shinden.eu/cdn1/avatars/225x350/85.jpg",
                },
                Fields = new List<RichMessageField>
                {
                    new RichMessageField
                    {
                        IsInline = true,
                        Name = "Max 256 characters",
                        Value = "Max 1024 characters",
                    },
                    new RichMessageField
                    {
                        Value = "25",
                        IsInline = false,
                        Name = "Max fields count",
                    },
                },
            };

            return Ok(result);
        }
    }
}