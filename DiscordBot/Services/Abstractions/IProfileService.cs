using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Services.Abstractions
{
    public interface IProfileService
    {
        Task RomoveUserColorAsync(SocketGuildUser user);
    }
}
