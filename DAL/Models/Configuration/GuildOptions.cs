using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models.Configuration
{
    public class GuildOptions
    {
        private GuildOptions() { }

        public GuildOptions(ulong id, long safariLimit)
        {
            Id = id;
            SafariLimit = safariLimit;
        }

        /// <summary>
        /// The Discord guild (server) identifier.
        /// </summary>
        public ulong Id { get; set; }
        public ulong MuteRole { get; set; }
        public ulong ModMuteRole { get; set; }
        public ulong UserRole { get; set; }
        public ulong AdminRole { get; set; }
        public ulong GlobalEmotesRole { get; set; }
        public ulong WaifuRole { get; set; }
        public ulong NotificationChannel { get; set; }
        public ulong RaportChannel { get; set; }
        public ulong QuizChannel { get; set; }
        public ulong ToDoChannel { get; set; }
        public ulong NsfwChannel { get; set; }
        public ulong LogChannel { get; set; }
        public ulong GreetingChannel { get; set; }

        [StringLength(50)]
        public string WelcomeMessage { get; set; }
        
        [StringLength(50)]
        public string WelcomeMessagePW { get; set; }

        [StringLength(50)]
        public string GoodbyeMessage { get; set; }
        public long SafariLimit { get; set; }
        public bool Supervision { get; set; }
        public bool ChaosMode { get; set; }

        [StringLength(50)]
        public string Prefix { get; set; }

        public virtual WaifuConfiguration WaifuConfig { get; set; }

        public virtual ICollection<WithoutSupervisionChannel> ChannelsWithoutSupervision { get; set; }
        public virtual ICollection<WithoutMessageCountChannel> IgnoredChannels { get; set; }
        public virtual ICollection<WithoutExpChannel> ChannelsWithoutExp { get; set; }
        public virtual ICollection<CommandChannel> CommandChannels { get; set; }
        public virtual ICollection<ModeratorRoles> ModeratorRoles { get; set; }
        public virtual ICollection<LevelRole> RolesPerLevel { get; set; }
        public virtual ICollection<SelfRole> SelfRoles { get; set; }
        public virtual ICollection<Raport> Raports { get; set; }
        public virtual ICollection<MyLand> Lands { get; set; }
    }
}
