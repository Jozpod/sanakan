using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web
{
    public interface IUserContext
    {
        /// <summary>
        /// Gets user identifier in Discord.
        /// </summary>
        ulong? DiscordId { get;  }

        bool HasWebpageClaim();
    }
}
