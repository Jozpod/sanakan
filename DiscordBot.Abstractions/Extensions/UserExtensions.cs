using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Abstractions.Extensions
{
    public static class UserExtensions
    {
        public static bool IsBotOrWebhook(this IUser user)
        {
#if RELEASE 
            return user.IsBot || user.IsWebhook;
#elif TEST
            return user.IsBot || user.IsWebhook;
#else
            return false;
#endif
        }
    }
}
