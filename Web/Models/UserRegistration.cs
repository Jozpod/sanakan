using Sanakan.Models;
using System.Collections.Generic;

namespace Sanakan.Web.Models
{
    /// <summary>
    /// Describes user registration model.
    /// </summary>
    public class UserRegistration
    {
        /// <summary>
        /// Specifies whether the user is super admin.
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        /// <summary>
        /// The user identifier in Shinden forum.
        /// </summary>
        public ulong ForumUserId { get; set; }

        /// <summary>
        /// The user identifier in Discord.
        /// </summary>
        public ulong DiscordUserId { get; set; }

        /// <summary>
        /// The username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The collection of user ranks in forum.
        /// </summary>
        public List<ForumUserGroup> ForumGroupsId { get; set; } = new List<ForumUserGroup>();
    }
}
