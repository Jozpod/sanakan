﻿using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public class AcceptSession : InteractionSession
    {
        private readonly AcceptSessionPayload _payload;
        public class AcceptSessionPayload
        {
            public ulong MessageId { get; set; }

            public IMessageChannel Channel { get; set; } = null;

            public IUser Bot { get; set; } = null;

            public TimeSpan Duration { get; set; }

            public IRole UserRole { get; set; } = null;

            public IRole MuteRole { get; set; } = null;

            public IGuildUser User { get; set; } = null;

            public ITextChannel NotifyChannel { get; set; } = null;
        }

        public AcceptSession(
            ulong ownerId,
            DateTime createdOn,
            AcceptSessionPayload payload) : base(
            ownerId,
            createdOn,
            TimeSpan.FromMinutes(2),
            Discord.Commands.RunMode.Sync,
            SessionExecuteCondition.AllEvents)
        {
            _payload = payload;
        }

        public override async Task ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            var iconConfiguration = serviceProvider.GetRequiredService<IIconConfiguration>();

            try
            {
                IsRunning = true;

                if (context.Message.Id != _payload.MessageId)
                {
                    return;
                }

                if (context.UserId != OwnerId)
                {
                    return;
                }

                var channel = _payload.Channel;
                var userMessage = await channel.GetMessageAsync(_payload.MessageId) as IUserMessage;

                if (userMessage == null)
                {
                    return;
                }

                var reaction = context.AddReaction ?? context.RemoveReaction;

                if (reaction.Emote.Equals(iconConfiguration.Decline)
                    || !reaction.Emote.Equals(iconConfiguration.Accept))
                {
                    return;
                }

                var moderatorService = serviceProvider.GetRequiredService<IModeratorService>();

                if (userMessage != null)
                {
                    await userMessage.DeleteAsync();
                }

                var reason = "Chciał to dostał :)";
                var info = await moderatorService.MuteUserAsync(
                    _payload.User,
                    _payload.MuteRole,
                    null,
                    _payload.UserRole,
                    _payload.Duration,
                    reason);
                await moderatorService.NotifyAboutPenaltyAsync(_payload.User, _payload.NotifyChannel, info, "Sanakan");

                var content = $"{_payload.User.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
                await channel.SendMessageAsync(embed: content);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}