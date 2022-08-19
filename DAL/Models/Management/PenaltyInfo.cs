using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models.Management
{
    /// <summary>
    /// Describes exercised penalty on a given user caused by ToS/ToU violation.
    /// </summary>
    public class PenaltyInfo
    {
        public PenaltyInfo()
        {
            Roles = new Collection<OwnedRole>();
        }

        /// <summary>
        /// The unique identifier.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The Discord guild (server) identifer.
        /// </summary>
        public ulong GuildId { get; set; }

        /// <summary>
        /// Describes the reason why penalty was given to user on Discord.
        /// </summary>
        [StringLength(100)]
        public string? Reason { get; set; }

        /// <summary>
        /// The type of penalty.
        /// </summary>
        public PenaltyType Type { get; set; }

        /// <summary>
        /// The datetime when penalty started.
        /// </summary>
        public DateTime StartedOn { get; set; }

        /// <summary>
        /// The time span of penalty applied to user on Discord.
        /// </summary>
        public TimeSpan Duration { get; set; }

        public virtual ICollection<OwnedRole> Roles { get; set; }
    }
}
