using System;

namespace Sanakan.DAL.Models.Analytics
{
    public class TransferAnalytics
    {
        public ulong Id { get; set; }

        /// <summary>
        /// Amount of TC transfered.
        /// </summary>
        public ulong Value { get; set; }

        public DateTime CreatedOn { get; set; }

        public ulong DiscordUserId { get; set; }

        public ulong ShindenUserId { get; set; }

        public TransferSource Source { get; set; }
    }
}