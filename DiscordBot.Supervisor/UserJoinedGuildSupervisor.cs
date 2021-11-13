using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor
{
    internal class UserJoinedGuildSupervisor : IUserJoinedGuildSupervisor
    {
        private readonly IOptionsMonitor<SupervisorConfiguration> _supervisorConfiguration;
        private readonly IDictionary<string, Entry> _entries;
        private readonly TimeSpan _timeIntervalBetweenUserGuildJoins;
        private readonly ISystemClock _systemClock;

        public UserJoinedGuildSupervisor(
            IOptionsMonitor<SupervisorConfiguration> supervisorConfiguration,
            ISystemClock systemClock)
        {
            _supervisorConfiguration = supervisorConfiguration;
            _systemClock = systemClock;
            _entries = new Dictionary<string, Entry>(100);
            _timeIntervalBetweenUserGuildJoins = _supervisorConfiguration.CurrentValue.TimeIntervalBetweenUserGuildJoins;
        }

        internal class Entry
        {
            public bool IsRaid { get; set; }
            public ISet<ulong> UserIds { get; set; }
            public uint OccurenceCount { get; set; }
            public DateTime ModifiedOn { get; set; }
        }

        public IEnumerable<ulong> GetUsersToBanCauseRaid(ulong guildId, string username, ulong userId)
        {
            var key = $"{guildId}-{username}";
            var utcNow = _systemClock.UtcNow;
            var hasExpired = true;
            Entry entry;

            if (_entries.TryGetValue(key, out entry))
            {
                hasExpired = utcNow - entry.ModifiedOn > _timeIntervalBetweenUserGuildJoins;

                if (hasExpired)
                {
                    entry.ModifiedOn = utcNow;
                    entry.IsRaid = false;
                    entry.UserIds.Clear();
                }

                entry.UserIds.Add(userId);
            }
            else
            {
                entry = new Entry
                {
                    ModifiedOn = utcNow,
                    OccurenceCount = 1,
                    UserIds = new HashSet<ulong>() { userId },
                };

                _entries[key] = entry;
            }

            if(entry.UserIds.Count > _supervisorConfiguration.CurrentValue.SameUsernameLimit)
            {
                entry.IsRaid = true;
            }

            if (hasExpired || !entry.IsRaid)
            {
                return Enumerable.Empty<ulong>();
            }

            var result = new List<ulong>(entry.UserIds);
            entry.UserIds.Clear();

            return result;
        }

        public void Refresh()
        {
            var utcNow = _systemClock.UtcNow;
            foreach (var entry in _entries.Values)
            {
                if(utcNow - entry.ModifiedOn > _timeIntervalBetweenUserGuildJoins)
                {
                    entry.ModifiedOn = utcNow;
                    entry.UserIds.Clear();
                    entry.IsRaid = false;
                }
            }
        }
    }
}
