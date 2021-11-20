using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.DiscordBot.Abstractions;
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
            public IMessage Message { get; set; }
            public IUser Bot { get; set; }
            public TimeSpan Duration { get; set; }
            public IRole UserRole { get; set; }
            public IRole MuteRole { get; set; }
            public IGuildUser User { get; set; }
            public ITextChannel NotifChannel { get; set; }
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
            try
            {
                IsRunning = true;
                if (_payload.Message == null)
                {
                    return;
                }

                if (context.Message.Id != _payload.Message.Id)
                {
                    return;
                }

                if (context.User.Id != OwnerId)
                {
                    return;
                }

                var userMessage = await _payload.Message.Channel.GetMessageAsync(_payload.Message.Id) as IUserMessage;

                if (userMessage == null)
                {
                    return;
                }

                var reaction = context.AddReaction ?? context.RemoveReaction;

                if (reaction.Emote.Equals(Emojis.DeclineEmote))
                {
                    return;
                }

                if (!reaction.Emote.Equals(Emojis.Checked))
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
                await moderatorService.NotifyAboutPenaltyAsync(_payload.User, _payload.NotifChannel, info, "Sanakan");

                var content = $"{_payload.User.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
                await _payload.Message.Channel.SendMessageAsync("", embed: content);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
