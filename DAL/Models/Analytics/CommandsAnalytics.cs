using System;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models.Analytics
{
    public class CommandsAnalytics
    {
        /// <summary>
        /// The unique identifier.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The Discord guild (server) identifier.
        /// </summary>
        public ulong GuildId { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// The command name which user typed in Discord.
        /// </summary>
        [StringLength(50)]
        [Required]
        public string CommandName { get; set; } = string.Empty;

        /// <summary>
        /// The command parameters which user provided with command in Discord.
        /// </summary>
        [StringLength(50)]
        public string? CommandParameters { get; set; }
    }
}