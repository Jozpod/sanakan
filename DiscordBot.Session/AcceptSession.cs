using Discord;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.DiscordBot.Abstractions.Configuration;
using Sanakan.DiscordBot.Abstractions.Extensions;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Session.Abstractions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public class AcceptSession : InteractionSession
    {
        private readonly IUser _bot;
        private readonly IGuildUser _user;
        private readonly IUserMessage _userMessage;
        private readonly IMessageChannel _messageChannel;
        private readonly IMessageChannel _notifyChannel;
        private readonly IRole _userRole;
        private readonly IRole _muteRole;
        private IIconConfiguration _iconConfiguration = null;

        public AcceptSession(
            ulong ownerId,
            DateTime createdOn,
            IUser bot,
            IGuildUser user,
            IUserMessage userMessage,
            IMessageChannel messageChannel,
            IMessageChannel notifyChannel,
            IRole userRole,
            IRole muteRole) : base(
            ownerId,
            createdOn,
            TimeSpan.FromMinutes(2),
            Discord.Commands.RunMode.Sync,
            SessionExecuteCondition.AllEvents)
        {
            _bot = bot;
            _user = user;
            _userMessage = userMessage;
            _messageChannel = messageChannel;
            _notifyChannel = notifyChannel;
            _userRole = userRole;
            _muteRole = muteRole;
        }

        public override async Task<bool> ExecuteAsync(
            SessionContext context,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default)
        {
            _iconConfiguration = serviceProvider.GetRequiredService<IIconConfiguration>();
            var moderatorService = serviceProvider.GetRequiredService<IModeratorService>();
            var randomNumberGenerator = serviceProvider.GetRequiredService<IRandomNumberGenerator>();

            try
            {
                IsRunning = true;

                if (context.Message.Id != _userMessage.Id)
                {
                    return false;
                }

                if (OwnerIds.Any(pr => pr == context.UserId))
                {
                    return false;
                }

                var reaction = context.AddReaction ?? context.RemoveReaction;

                if (reaction.Emote.Equals(_iconConfiguration.Decline)
                    || !reaction.Emote.Equals(_iconConfiguration.Accept))
                {
                    return false;
                }

                const int daysInYear = 365;
                var duration = TimeSpan.FromDays(randomNumberGenerator.GetRandomValue(daysInYear) + 1);
                await _userMessage.DeleteAsync();

                var reason = "Chciał to dostał :)";
                var info = await moderatorService.MuteUserAsync(
                    _user,
                    _muteRole,
                    null,
                    _userRole,
                    duration,
                    reason);
                await moderatorService.NotifyAboutPenaltyAsync(_user, _notifyChannel, info, "Sanakan");

                var content = $"{_user.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
                await _messageChannel.SendMessageAsync(embed: content);
                return true;
            }
            finally
            {
                IsRunning = false;
            }
        }

        public override async ValueTask DisposeAsync()
        {
            try
            {
                await _userMessage.RemoveAllReactionsAsync();
            }
            catch (Exception)
            {
                _iconConfiguration ??= ServiceProvider.GetRequiredService<IIconConfiguration>();

                await _userMessage.RemoveReactionsAsync(_bot, _iconConfiguration.AcceptDecline);
            }
        }
    }
}
