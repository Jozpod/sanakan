using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot;
using Sanakan.Extensions;

namespace Sanakan.Services
{
    public class DeletedLog
    {
        private readonly IDiscordSocketClientAccessor _discordSocketClientAccessor;
        private readonly IOptionsMonitor<SanakanConfiguration> _config;
        private readonly IServiceScopeFactory _scopeFactory;

        public DeletedLog(
            IDiscordSocketClientAccessor discordSocketClientAccessor,
            IOptionsMonitor<SanakanConfiguration> config,
            IServiceScopeFactory scopeFactory)
        {
            _discordSocketClientAccessor = discordSocketClientAccessor;
            _config = config;
            _scopeFactory = scopeFactory;

            var client = _discordSocketClientAccessor.Client;

            if(client == null)
            {
                throw new Exception("Client not available.");
            }

            client.MessageDeleted += HandleDeletedMsgAsync;
            client.MessageUpdated += HandleUpdatedMsgAsync;
        }

        private async Task HandleUpdatedMsgAsync(
            Cacheable<IMessage, ulong> oldMessage,
            SocketMessage newMessage,
            ISocketMessageChannel channel)
        {
            if (!oldMessage.HasValue)
            {
                return;
            }


            if (newMessage.Author.IsBot || newMessage.Author.IsWebhook)
            {
                return;
            }

            if (oldMessage.Value.Content.Equals(newMessage.Content, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            if (newMessage.Channel is SocketGuildChannel gChannel)
            {
                if (_config.CurrentValue.BlacklistedGuilds.Any(x => x == gChannel.Guild.Id))
                {
                    return;
                }

                _ = Task.Run(async () =>
                {
                    await LogMessageAsync(gChannel, oldMessage.Value, newMessage);
                });
            }

            await Task.CompletedTask;
        }

        private async Task HandleDeletedMsgAsync(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            if (!message.HasValue)
            {
                return;
            }

            if (message.Value.Author.IsBot || message.Value.Author.IsWebhook)
            {
                return;
            }

            if (message.Value.Content.Length < 4 && message.Value.Attachments.Count < 1)
            {
                return;
            }

            if (message.Value.Channel is SocketGuildChannel gChannel)
            {
                if (_config.CurrentValue.BlacklistedGuilds.Any(x => x == gChannel.Guild.Id))
                {
                    return;
                }

                _ = Task.Run(async () =>
                {
                    await LogMessageAsync(gChannel, message.Value);
                });
            }

            await Task.CompletedTask;
        }

        private async Task LogMessageAsync(SocketGuildChannel channel, IMessage oldMessage, IMessage? newMessage = null)
        {
            using var serviceScope = _scopeFactory.CreateScope();
            var guildConfigRepository = serviceScope.ServiceProvider.GetRequiredService<IGuildConfigRepository>();

            if (oldMessage.Content.IsEmotikunEmote() && newMessage == null)
            {
                return;
            }

            var config = await guildConfigRepository.GetCachedGuildFullConfigAsync(channel.Guild.Id);

            if (config == null)
            {
                return;
            }

            var textChannel = channel.Guild.GetTextChannel(config.LogChannelId);
            
            if (textChannel == null)
            {
                return;
            }

            var jump = (newMessage == null) ? "" : $"{newMessage.GetJumpUrl()}";
            await textChannel.SendMessageAsync(jump, embed: BuildMessage(oldMessage, newMessage));
        }

        private Embed BuildMessage(IMessage oldMessage, IMessage newMessage)
        {
            string content = (newMessage == null) ? oldMessage.Content
                : $"**Stara:**\n{oldMessage.Content}\n\n**Nowa:**\n{newMessage.Content}";

            return new EmbedBuilder
            {
                Color = (newMessage == null) ? EMType.Warning.Color() : EMType.Info.Color(),
                Author = new EmbedAuthorBuilder().WithUser(oldMessage.Author, true),
                Fields = GetFields(oldMessage, newMessage == null),
                Description = content.TrimToLength(1800),
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
    }
}
