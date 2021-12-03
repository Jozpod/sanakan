using Sanakan.Models;
using System.Collections.Generic;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// Struktura rejestracji użytkownika
    /// </summary>
    public class UserRegistration
    {
        /// <summary>
        /// Czy posiada uprawnienia su
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
        public string Username { get; set; }

        /// <summary>
        /// The collection of user ranks in forum.
        /// </summary>
        public List<ForumUserGroup> ForumGroupsId { get; set; } = new List<ForumUserGroup>();
    }
}
