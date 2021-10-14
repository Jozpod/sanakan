using System;

namespace Sanakan.DAL.Models.Analytics
{
    public class TransferAnalytics
    {
        public ulong Id { get; set; }
        public long Value { get; set; }
        public DateTime Date { get; set; }
        public ulong DiscordId { get; set; }
        public ulong ShindenId { get; set; }
        public TransferSource Source { get; set; }
    }
}