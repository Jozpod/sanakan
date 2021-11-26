using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Supervisor
{
    internal class UserMessageSupervisor : IUserMessageSupervisor
    {
        private readonly ILogger<UserMessageSupervisor> _logger;
        private readonly IOptionsMonitor<DiscordConfiguration> _discordConfiguration;
        private readonly IOptionsMonitor<SupervisorConfiguration> _supervisorConfiguration;
        private readonly ISystemClock _systemClock;
        private readonly IFileSystem _fileSystem;
        private readonly IDictionary<string, UserEntry> _entries;
        private readonly IFileSystemWatcher _fileSystemWatcher;
        private Task<string[]> _disallowedUrls;
        private readonly TimeSpan _messageExpiry;
        private readonly TimeSpan _timeIntervalBetweenMessages;
        private readonly Regex _urlsRegex;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public UserMessageSupervisor(
            ILogger<UserMessageSupervisor> logger,
            IOptionsMonitor<DiscordConfiguration> discordConfiguration,
            IOptionsMonitor<SupervisorConfiguration> supervisorConfiguration,
            ISystemClock systemClock,
            IFileSystem fileSystem,
            IFileSystemWatcherFactory fileSystemWatcherFactory,
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _discordConfiguration = discordConfiguration;
            _supervisorConfiguration = supervisorConfiguration;
            _entries = new Dictionary<string, UserEntry>(100);
            _systemClock = systemClock;
            _fileSystem = fileSystem;
            _urlsRegex = new Regex(
                @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var fileName = "disallowed-urls.txt";
            _fileSystemWatcher = fileSystemWatcherFactory.Create(new FileSystemWatcherOptions
            {
                Path = hostEnvironment.ContentRootPath,
                Filter = fileName,
            });
            _fileSystemWatcher.Changed += UrlsChanged;

            _disallowedUrls = _fileSystem.ReadAllLinesAsync(fileName);
            _messageExpiry = supervisorConfiguration.CurrentValue.MessageExpiry;
            _timeIntervalBetweenMessages = supervisorConfiguration.CurrentValue.TimeIntervalBetweenMessages;
        }

        private async void UrlsChanged(object sender, FileSystemEventArgs eventArgs)
        {
            try
            {
                await _semaphore.WaitAsync();
                _disallowedUrls = _fileSystem.ReadAllLinesAsync(eventArgs.Name);
                _semaphore.Release();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while updating urls", ex);
            }
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

        public async Task<SupervisorAction> MakeDecisionAsync(ulong guildId, ulong userId, string content, bool lessSeverePunishment)
        {
            var hasDisallowedUrl = false;

            await _semaphore.WaitAsync();
            hasDisallowedUrl = (await _disallowedUrls).Any(pr => content.Contains(content));
            _semaphore.Release();

            var key = $"{guildId}-{userId}";
            var hasUrlsInMessage = _urlsRegex.Matches(content).Select(x => x.Value).Any();
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
                    if (utcNow - messageEntry.ReceivedOn > _messageExpiry)
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

            var configuration = _supervisorConfiguration.CurrentValue;
            var userMessageLimit = configuration.MessagesLimit;
            var messageLimit = configuration.MessageLimit;
            var commandLimit = configuration.MessageCommandLimit;

            if (_discordConfiguration.CurrentValue.BanForUrlSpam)
            {
                lessSeverePunishment &= !messageEntry.HasDisallowedUrl;
            }

            if (messageEntry.IsCommand)
            {
                userMessageLimit += commandLimit;
                messageLimit += commandLimit;
            }

            if (!lessSeverePunishment)
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
