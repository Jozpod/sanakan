using System;
using System.Collections.Generic;

namespace Sanakan.DAL.Models.Management
{
    public class PenaltyInfo
    {
        public ulong Id { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong User { get; set; }

        /// <summary>
        /// The Discord guild identifer.
        /// </summary>
        public ulong Guild { get; set; }
        public string Reason { get; set; }
        public PenaltyType Type { get; set; }
        public DateTime StartDate { get; set; }
        public long DurationInHours { get; set; }

        public virtual ICollection<OwnedRole> Roles { get; set; }
    }
}
