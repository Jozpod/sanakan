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

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong MuteRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong ModMuteRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong UserRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong AdminRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong GlobalEmotesRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong WaifuRoleId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong NotificationChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong RaportChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong QuizChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong ToDoChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong NsfwChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong LogChannelId { get; set; }

        /// <summary>
        /// The Discord channel identifier.
        /// </summary>
        public ulong GreetingChannelId { get; set; }

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
