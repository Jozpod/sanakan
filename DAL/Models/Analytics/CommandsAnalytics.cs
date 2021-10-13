using System;

namespace Sanakan.DAL.Models.Analytics
{
    public class CommandsAnalytics
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime Date { get; set; }
        public string CmdName { get; set; }
        public string CmdParams { get; set; }
    }
}