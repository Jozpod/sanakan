using Sanakan.Common.Models;
using System.Collections.Generic;

namespace Sanakan.Common.Configuration
{
    public class DatabaseSeedConfiguration
    {
        public bool Enabled { get; set; }

        public List<GuildSeedConfiguration> Guilds { get; set; } = new();

        public class GuildSeedConfiguration
        {
            public ulong Id { get; set; }

            public ulong? SafariLimit { get; set; }

            public ulong? LogChannelId { get; set; }

            public ulong? NsfwChannelId { get; set; }

            public ulong? TodoChannelId { get; set; }

            public ulong? ReportChannelId { get; set; }

            public ulong? NotificationChannelId { get; set; }

            public ulong? GreetingChannelId { get; set; }

            public ulong? NonSupChannelId { get; set; }

            public ulong? NonExpChannelId { get; set; }

            public ulong? IgnoredChannelId { get; set; }

            public ulong? TrashCommandsChannelId { get; set; }

            public ulong? CommandChannelId { get; set; }

            public ulong? CommandWaifuChannelId { get; set; }

            public ulong? FightWaifuChannelId { get; set; }

            public ulong? SafariWaifuChannelId { get; set; }

            public ulong? AdminRoleId { get; set; }

            public ulong? WaifuRoleId { get; set; }

            public ulong? UserRoleId { get; set; }

            public ulong? MuteRoleId { get; set; }

            public ulong? ModMuteRoleId { get; set; }

            public ulong? GlobalEmotesRoleId { get; set; }

            public List<RolesPerLevelSeedConfiguration> RolesPerLevel { get; set; } = new();

            public List<UserSeedConfiguration> Users { get; set; } = new();

            public string? WelcomeMessage { get; set; }
        }

        public class RolesPerLevelSeedConfiguration
        {
            public ulong Level { get; set; }

            public ulong RoleId { get; set; }
        }

        public class UserSeedConfiguration
        {
            public ulong Id { get; set; }

            public ulong? Level { get; set; }

            public ulong? ShindenId { get; set; }

            public double? Karma { get; set; }

            public double? ExperiencePercentage { get; set; }

            public int? NumberOfCards { get; set; }

            public int? ActiveCards { get; set; }

            public double? DefaultAffection { get; set; }

            public int? WishListItems { get; set; }

            public ulong? NumberOfItems { get; set; }

            public ulong? ScCount { get; set; }

            public ulong? TcCount { get; set; }

            public ulong? AcCount { get; set; }

            public ulong? PVPCoins { get; set; }

            public ProfileType? ProfileType { get; set; }

            public ulong? MessagesCount { get; set; }

            public ulong? CommandsCount { get; set; }
        }
    }
}
