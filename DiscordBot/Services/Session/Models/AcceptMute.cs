using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Extensions;
using System.Threading.Tasks;

namespace Sanakan.Services.Session.Models
{
    public class AcceptMute : IAcceptActions
    {
        public IMessage Message { get; set; }
        public SocketRole UserRole { get; set; }
        public SocketRole MuteRole { get; set; }
        public SocketGuildUser User { get; set; }
        public Moderator Moderation { get; set; }
        public SocketTextChannel NotifChannel { get; set; }

        private readonly object _config;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public AcceptMute(
            IOptions<object> config,
            IRandomNumberGenerator _randomNumberGenerator)
        {
            _config = config;
        }

        public async Task<bool> OnAccept(SessionContext context)
        {
            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.DeleteAsync();
            }

            const int daysInYear = 365;
            const int hoursInDay = 24;
            var duration = (_randomNumberGenerator.GetRandomValue(daysInYear) * hoursInDay) + hoursInDay;

            var info = await Moderation.MuteUserAysnc(
                User,
                MuteRole,
                null,
                UserRole,
                duration,
                "Chciał to dostał :)");
            await Moderation.NotifyAboutPenaltyAsync(User, NotifChannel, info, "Sanakan");

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
