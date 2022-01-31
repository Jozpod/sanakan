using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services
{
    public class AuditService : IAuditService
    {
        private readonly ILogger _logger;
        private readonly IDiscordClientAccessor _discordClientAccessor;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AuditService(
            ILogger<AuditService> logger,
            IDiscordClientAccessor discordClientAccessor,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IServiceScopeFactory serviceScopeFactory)
        {
            _discordClientAccessor = discordClientAccessor;
            _logger = logger;
            _discordConfiguration = discordConfiguration;
            _serviceScopeFactory = serviceScopeFactory;
            _discordClientAccessor.Log += OnLog;
            _discordClientAccessor.MessageDeleted += HandleDeletedMessageAsync;
            _discordClientAccessor.MessageUpdated += HandleUpdatedMessageAsync;
        }

        private IGuild? ValidateMessageAndTryGetGuild(IMessage message)
        {
            var user = message.Author;

            if (user.IsBotOrWebhook())
            {
                return null;
            }

            var guildChannel = message.Channel as IGuildChannel;

            if (guildChannel == null)
            {
                return null;
            }

            var guild = guildChannel.Guild;
            if (_discordConfiguration.CurrentValue.BlacklistedGuilds.Contains(guild.Id))
            {
                return null;
            }

            return guild;
        }

        private async Task HandleUpdatedMessageAsync(
            Cacheable<IMessage, ulong> oldMessage,
            IMessage newMessage,
            ISocketMessageChannel channel)
        {
            if (!oldMessage.HasValue)
            {
                return;
            }

            if (oldMessage.Value.Content.Equals(newMessage.Content, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            var guild = ValidateMessageAndTryGetGuild(newMessage);

            if (guild == null)
            {
                return;
            }

            await LogMessageAsync(guild, oldMessage.Value, newMessage);
        }

        private async Task HandleDeletedMessageAsync(Cacheable<IMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> channel)
        {
            if (!cachedMessage.HasValue)
            {
                return;
            }

            var message = cachedMessage.Value;

            var guild = ValidateMessageAndTryGetGuild(message);

            if (guild == null)
            {
                return;
            }

            if (message.Content.Length < 4 && message.Attachments.Count < 1)
            {
                return;
            }

            await LogMessageAsync(guild, message);
        }

        private async Task LogMessageAsync(IGuild guild, IMessage oldMessage, IMessage? newMessage = null)
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();

            if (oldMessage.Content.IsEmotikunEmote() && newMessage == null)
            {
                return;
            }

            var config = await guildConfigRepository.GetCachedById(guild.Id);

            if (config == null)
            {
                return;
            }

            var textChannel = await guild.GetChannelAsync(config.LogChannelId) as IMessageChannel;

            if (textChannel == null)
            {
                return;
            }

            try
            {
                var jumpUrl = (newMessage == null) ? "" : $"{newMessage.GetJumpUrl()}";
                var embed = BuildMessage(oldMessage, newMessage);
                await textChannel.SendMessageAsync(jumpUrl, embed: embed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging");
                throw;
            }
        }

        private Embed BuildMessage(IMessage oldMessage, IMessage? newMessage)
        {
            Color color;
            string content;

            if(newMessage == null)
            {
                color = EMType.Warning.Color();
                content = oldMessage.Content;
            }
            else
            {
                color = EMType.Info.Color();
                content = $"**Stara:**\n{oldMessage.Content}\n\n**Nowa:**\n{newMessage.Content}";
            }

            return new EmbedBuilder
            {
                Color = color,
                Author = new EmbedAuthorBuilder().WithUser(oldMessage.Author, true),
                Fields = GetFields(oldMessage, newMessage == null),
                Description = content.ElipseTrimToLength(1800),
            }.Build();
        }

        private List<EmbedFieldBuilder> GetFields(IMessage message, bool deleted)
        {
            var fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = deleted ? "Napisano:" : "Edytowano:",
                    Value = message.GetLocalCreatedAtShortDateTime()
                },
                new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Kanał:",
                    Value = message.Channel.Name
                }
            };

            if (deleted)
            {
                fields.Add(new EmbedFieldBuilder
                {
                    IsInline = true,
                    Name = "Załączniki:",
                    Value = message.Attachments?.Count
                });
            }

            return fields;
        }

        private Task OnLog(LogMessage log)
        {
            switch (log.Severity)
            {
                case LogSeverity.Debug:
                    _logger.LogDebug(log.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(log.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogDebug(log.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(log.Message);
                    break;
                case LogSeverity.Critical:
                    _logger.LogCritical(log.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(log.Message);
                    break;
                default:
                    _logger.LogInformation(log.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
