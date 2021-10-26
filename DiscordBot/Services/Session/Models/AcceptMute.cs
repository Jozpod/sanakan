using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Extensions;
using System;
using System.Threading.Tasks;

namespace Sanakan.Services.Session.Models
{
    public class AcceptMute : IAcceptActions
    {
        public IMessage Message { get; set; }
        public SocketRole UserRole { get; set; }
        public SocketRole MuteRole { get; set; }
        public SocketGuildUser User { get; set; }
        public SocketTextChannel NotifChannel { get; set; }

        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AcceptMute(
            IRandomNumberGenerator randomNumberGenerator,
            IServiceScopeFactory serviceScopeFactory)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<bool> OnAccept(SessionContext context)
        {
            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.DeleteAsync();
            }

            const int daysInYear = 365;
            const int hoursInDay = 24;
            var duration = TimeSpan.FromDays(_randomNumberGenerator.GetRandomValue(daysInYear) + 1);
            var reason = "Chciał to dostał :)";

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            var moderatorService = serviceProvider.GetRequiredService<DiscordBot.Services.Abstractions.IModeratorService>();
             
            var info = await moderatorService.MuteUserAysnc(
                User,
                MuteRole,
                null,
                UserRole,
                duration,
                reason);
            await moderatorService.NotifyAboutPenaltyAsync(User, NotifChannel, info, "Sanakan");

            var content = $"{User.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build();
            await Message.Channel.SendMessageAsync("", embed: content);
            return true;
        }

        public async Task<bool> OnDecline(SessionContext context)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
