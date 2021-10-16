using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
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

        public AcceptMute(IOptions<object> config)
        {
            _config = config;
        }

        public async Task<bool> OnAccept(SessionContext context)
        {
            if (await Message.Channel.GetMessageAsync(Message.Id) is IUserMessage msg)
            {
                await msg.DeleteAsync();
            }

            var info = await Moderation.MuteUserAysnc(
                User, MuteRole, null, UserRole, mdb, (Fun.GetRandomValue(365) * 24) + 24, "Chciał to dostał :)");
            await Moderation.NotifyAboutPenaltyAsync(User, NotifChannel, info, "Sanakan");

            await Message.Channel.SendMessageAsync("", embed: $"{User.Mention} został wyciszony.".ToEmbedMessage(EMType.Success).Build());
            return true;
        }

        public async Task<bool> OnDecline(SessionContext context)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
