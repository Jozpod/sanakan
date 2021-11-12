using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Sanakan.DAL.Models.Configuration
{
    /// <summary>
    /// Describes the Discord guild (server) options.
    /// </summary>
    public class GuildOptions
    {
        private GuildOptions()
        {
            ChannelsWithoutSupervision = new Collection<WithoutSupervisionChannel>();
            IgnoredChannels = new Collection<WithoutMessageCountChannel>();
            ChannelsWithoutExperience = new Collection<WithoutExpChannel>();
            CommandChannels = new Collection<CommandChannel>();
            ModeratorRoles = new Collection<ModeratorRoles>();
            RolesPerLevel = new Collection<LevelRole>();
            SelfRoles = new Collection<SelfRole>();
            Raports = new Collection<Raport>();
            Lands = new Collection<MyLand>();
        }

        public GuildOptions(ulong id, ulong safariLimit)
        {
            Id = id;
            SafariLimit = safariLimit;
            ChannelsWithoutSupervision = new Collection<WithoutSupervisionChannel>();
            IgnoredChannels = new Collection<WithoutMessageCountChannel>();
            ChannelsWithoutExperience = new Collection<WithoutExpChannel>();
            CommandChannels = new Collection<CommandChannel>();
            ModeratorRoles = new Collection<ModeratorRoles>();
            RolesPerLevel = new Collection<LevelRole>();
            SelfRoles = new Collection<SelfRole>();
            Raports = new Collection<Raport>();
            Lands = new Collection<MyLand>();
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
        public ulong? UserRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong? AdminRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong GlobalEmotesRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier.
        /// </summary>
        public ulong? WaifuRoleId { get; set; }

        /// <summary>
        /// The Discord channel identifier of text channel which is used for notifications.
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
        public string? WelcomeMessage { get; set; }
        
        [StringLength(50)]
        public string? WelcomeMessagePM { get; set; }

        [StringLength(50)]
        public string? GoodbyeMessage { get; set; }

        /// <summary>
        /// The number of Safari events which can happen during a day.
        /// </summary>
        public ulong SafariLimit { get; set; }
        public bool SupervisionEnabled { get; set; }
        public bool ChaosModeEnabled { get; set; }

        [StringLength(10)]
        public string? Prefix { get; set; }

        public virtual WaifuConfiguration? WaifuConfig { get; set; }

        public virtual ICollection<WithoutSupervisionChannel> ChannelsWithoutSupervision { get; set; }
        public virtual ICollection<WithoutMessageCountChannel> IgnoredChannels { get; set; }
        public virtual ICollection<WithoutExpChannel> ChannelsWithoutExperience { get; set; }
        public virtual ICollection<CommandChannel> CommandChannels { get; set; }
        public virtual ICollection<ModeratorRoles> ModeratorRoles { get; set; }
        public virtual ICollection<LevelRole> RolesPerLevel { get; set; }
        public virtual ICollection<SelfRole> SelfRoles { get; set; }
        public virtual ICollection<Raport> Raports { get; set; }
        public virtual ICollection<MyLand> Lands { get; set; }
    }
}
