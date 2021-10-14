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
        /// Id użytkownika forum
        /// </summary>
        public ulong ForumUserId { get; set; }
        /// <summary>
        /// Id użytkownika discord
        /// </summary>
        public ulong DiscordUserId { get; set; }
        /// <summary>
        /// Nazwa użytkownika na stronie
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Lista rang użytkownika na forum
        /// </summary>
        public List<ForumUserGroup> ForumGroupsId { get; set; }
    }
}
