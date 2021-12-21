using Sanakan.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.Common.Configuration
{
    public class DatabaseSeedConfiguration
    {
        public bool Enabled { get; set; }
        public IEnumerable<GuildSeedConfiguration> Guilds { get; set; } = Enumerable.Empty<GuildSeedConfiguration>();

        public class GuildSeedConfiguration
        {
            public ulong Id { get; set; }

            public ulong SafariLimit { get; set; }

            public ulong UserRoleId { get; set; }

            public ulong MuteRoleId { get; set; }

            public ulong MuteModRoleId { get; set; }

            public IEnumerable<UserSeedConfiguration> Users { get; set; } = Enumerable.Empty<UserSeedConfiguration>();
        }

        public class UserSeedConfiguration
        {
            public ulong Id { get; set; }

            public ulong Level { get; set; }

            public bool IsDeveloper { get; set; }

            public ulong NumberOfCards { get; set; }

            public ulong NumberOfItems { get; set; }

            public ulong ScCount { get; set; }

            public ulong TcCount { get; set; }

            public ulong AcCount { get; set; }

            public ulong ExperienceCount { get; set; }

            public ulong ProfileType { get; set; }

            public ulong MessagesCount { get; set; }

            public ulong CommandsCount { get; set; }
        }
    }
}
