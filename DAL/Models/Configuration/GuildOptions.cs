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
        public GuildOptions()
        {
            ChannelsWithoutSupervision = new Collection<WithoutSupervisionChannel>();
            IgnoredChannels = new Collection<WithoutMessageCountChannel>();
            ChannelsWithoutExperience = new Collection<WithoutExpChannel>();
            CommandChannels = new Collection<CommandChannel>();
            ModeratorRoles = new Collection<ModeratorRoles>();
            RolesPerLevel = new Collection<LevelRole>();
            SelfRoles = new Collection<SelfRole>();
            Raports = new Collection<Report>();
            Lands = new Collection<UserLand>();
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
            Raports = new Collection<Report>();
            Lands = new Collection<UserLand>();
        }

        /// <summary>
        /// The Discord guild (server) identifier.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// The Discord role identifier which is used for muting users.
        /// </summary>
        public ulong MuteRoleId { get; set; }

        /// <summary>
        /// The Discord role identifier which is used for muting moderators.
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
        /// The Discord channel identifier which is marked as "Not Safe For Work".
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

        [StringLength(2000)]
        public string? WelcomeMessage { get; set; }

        [StringLength(2000)]
        public string? WelcomeMessagePM { get; set; }

        [StringLength(2000)]
        public string? GoodbyeMessage { get; set; }

        /// <summary>
        /// The number of Safari events which can happen during a day.
        /// </summary>
        public ulong SafariLimit { get; set; }

        /// <summary>
        /// Specifies whether supervision is enabled.
        /// </summary>
        public bool SupervisionEnabled { get; set; }

        /// <summary>
        /// Specifies whether chaos mode is enabled.
        /// </summary>
        public bool ChaosModeEnabled { get; set; }

        /// <summary>
        /// The prefix which is used by Discord bot.
        /// </summary>
        [StringLength(10)]
        public string? Prefix { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public virtual WaifuConfiguration? WaifuConfig { get; set; }

        /// <summary>
        /// The list of Discord channels.
        /// </summary>
        public virtual ICollection<WithoutSupervisionChannel> ChannelsWithoutSupervision { get; set; }

        /// <summary>
        /// The list of Discord channels which are not moderated.
        /// </summary>
        public virtual ICollection<WithoutMessageCountChannel> IgnoredChannels { get; set; }
        
        /// <summary>
        /// The list of Discord channels which does not support experience gathering.
        /// </summary>
        public virtual ICollection<WithoutExpChannel> ChannelsWithoutExperience { get; set; }

        /// <summary>
        /// The list of Discord channels.
        /// </summary>
        public virtual ICollection<CommandChannel> CommandChannels { get; set; }

        /// <summary>
        /// The list of Discord roles.
        /// </summary>
        public virtual ICollection<ModeratorRoles> ModeratorRoles { get; set; }

        /// <summary>
        /// The list of Discord roles.
        /// </summary>
        public virtual ICollection<LevelRole> RolesPerLevel { get; set; }

        /// <summary>
        /// The list of Discord roles.
        /// </summary>
        public virtual ICollection<SelfRole> SelfRoles { get; set; }

        /// <summary>
        /// The list of raports.
        /// </summary>
        public virtual ICollection<Report> Raports { get; set; }

        /// <summary>
        /// The list of lands.
        /// </summary>
        public virtual ICollection<UserLand> Lands { get; set; }
    }
}
