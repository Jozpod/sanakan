using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Models
{
    /// <summary>
    /// Describes group forum roles.
    /// </summary>
    public enum ForumUserGroup
    {
        Unregistered = 1,
        User = 2,
        Administrator = 3,
        Moderator = 4,
        Banned = 8,
        SiteModerator = 19,
        GroupModerator = 20,
        DiscordAdministrator = 21,
        DiscordEmotes = 22,
        Developer = 23,
        DiscordPig = 24,
        Dev = 25
    }
}
