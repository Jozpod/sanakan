using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Supervisor;
using Sanakan.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor
{
    internal class UserMessageSupervisor : IUserMessageSupervisor
    {
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly ISystemClock _systemClock;
        private readonly IFileSystem _fileSystem;
        private readonly IDictionary<string, UserEntry> _entries;
        private IEnumerable<string> _disallowedUrls;
        private readonly TimeSpan _messageExpiry;
        private readonly TimeSpan _timeIntervalBetweenMessages;
        public readonly Regex _urlsRegex;

        public const int MessagesLimit = 12;
        public const int MessageLimit = 6;

        public UserMessageSupervisor(
            IOptionsMonitor<DiscordConfiguration> _discordConfiguration,
            ISystemClock systemClock,
            IFileSystem fileSystem)
        {
            _entries = new Dictionary<string, UserEntry>(100);
            _systemClock = systemClock;
            _fileSystem = fileSystem;
            _urlsRegex = new Regex(
                @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            _disallowedUrls = _fileSystem.ReadAllLinesAsync("disallowed-urls.txt").GetAwaiter().GetResult();
            _messageExpiry = TimeSpan.FromMinutes(2);
            _timeIntervalBetweenMessages = TimeSpan.FromSeconds(5);
        }

        internal class MessageEntry
        {
            public bool IsCommand { get; set; }
            public bool HasDisallowedUrl { get; set; }
            public uint OccurenceCount { get; set; }
            public DateTime ReceivedOn { get; set; }
        }

        public class UserEntry
        {
            public uint ShortDelayBetweenMessagesOccurenceCount { get; set; }
            public IDictionary<int, MessageEntry> Messages { get; set; }
            public DateTime ModifiedOn { get; set; }
        }

        //public bool HasExpired(ulong guildId, ulong userId)
        //{
        //    var key = $"{guildId}-{userId}";
        //    if (_entries.TryGetValue(key, out var entry))
        //    {
        //        return (entry.ModifiedOn - _systemClock.UtcNow) > _messageExpiry;
        //    }

        //    return false;
        //}

        public SupervisorAction MakeDecision(ulong guildId, ulong userId, string content, bool lessSeverePunishment)
        {
            var key = $"{guildId}-{userId}";
            var hasUrlsInMessage = _urlsRegex.Matches(content).Select(x => x.Value).Any();
            var hasDisallowedUrl = _disallowedUrls.Any(pr => content.Contains(content));
            hasDisallowedUrl &= hasUrlsInMessage;
            var utcNow = _systemClock.UtcNow;
            var messageKey = content.GetHashCode();
            MessageEntry messageEntry;

            if (_entries.TryGetValue(key, out var userEntry))
            {
                if (utcNow - userEntry.ModifiedOn > _timeIntervalBetweenMessages)
                {
                    userEntry.ModifiedOn = utcNow;
                    userEntry.ShortDelayBetweenMessagesOccurenceCount = 0;
                }
                else
                {
                    userEntry.ShortDelayBetweenMessagesOccurenceCount++;
                }

                if (userEntry.Messages.TryGetValue(messageKey, out messageEntry))
                {
                    if(utcNow - messageEntry.ReceivedOn > _messageExpiry)
                    {
                        messageEntry.ReceivedOn = utcNow;
                        messageEntry.OccurenceCount = 0;
                    }
                    else
                    {
                        messageEntry.OccurenceCount++;
                    }
                }
                else
                {
                    var isCommand = _discordConfiguration.CurrentValue.IsCommand(content);

                    messageEntry = new MessageEntry
                    {
                        IsCommand = isCommand,
                        HasDisallowedUrl = hasDisallowedUrl,
                        OccurenceCount = 1,
                        ReceivedOn = utcNow,
                    };

                    userEntry.Messages[messageKey] = messageEntry;
                }
            }
            else
            {
                userEntry = new UserEntry
                {
                    ShortDelayBetweenMessagesOccurenceCount = 0,
                    ModifiedOn = utcNow,
                    Messages = new Dictionary<int, MessageEntry>(),
                };

                var isCommand = _discordConfiguration.CurrentValue.IsCommand(content);

                messageEntry = new MessageEntry
                {
                    IsCommand = isCommand,
                    HasDisallowedUrl = hasDisallowedUrl,
                    OccurenceCount = 1,
                    ReceivedOn = utcNow,
                };

                userEntry.Messages[messageKey] = messageEntry;

                _entries[key] = userEntry;
            }

            var userMessageLimit = MessagesLimit;
            var messageLimit = MessageLimit;

            if (_discordConfiguration.CurrentValue.BanForUrlSpam)
            {
                lessSeverePunishment &= !messageEntry.HasDisallowedUrl;
            }

            if (messageEntry.IsCommand)
            {
                userMessageLimit += 2;
                messageLimit += 2;
            }

            if(!lessSeverePunishment)
            {
                userMessageLimit -= 2;
                messageLimit -= 2;
            }

            if (userEntry.ShortDelayBetweenMessagesOccurenceCount >= userMessageLimit
                || messageEntry.OccurenceCount > messageLimit)
            {
                if(lessSeverePunishment)
                {
                    return SupervisorAction.Mute;
                }
                return SupervisorAction.Ban;
            }

            if ((userEntry.ShortDelayBetweenMessagesOccurenceCount >= userMessageLimit - 1
                || messageEntry.OccurenceCount > messageLimit - 1)
                && lessSeverePunishment)
            {
                return SupervisorAction.Warn;
            }

            return SupervisorAction.None;
        }

        public void Refresh()
        {
            var utcNow = _systemClock.UtcNow;
            foreach (var entry in _entries.Values)
            {
                var hasExpired = (entry.ModifiedOn - utcNow) > _messageExpiry;

                if (hasExpired)
                {
                    entry.ModifiedOn = utcNow;
                    entry.Messages.Clear();
                    entry.ShortDelayBetweenMessagesOccurenceCount = 0;
                }
            }
        }
    }
}
