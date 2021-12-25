using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Abstractions
{
    public abstract class InteractionSession : IInteractionSession
    {
        public IEnumerable<ulong> OwnerIds { get; }

        private readonly DateTime _createdOn;
        private readonly TimeSpan _expiryPeriod;
        private DateTime _expiresOn;

        public DateTime ExpiresOn => _expiresOn;

        public Type Type { get; }
        public RunMode RunMode { get; }
        public SessionExecuteCondition SessionExecuteCondition { get; }
        public bool IsRunning { get; set; }
        public IServiceProvider ServiceProvider { get; set; } = null;

        public InteractionSession(
            ulong ownerId,
            DateTime createdOn,
            TimeSpan expiryPeriod,
            RunMode runMode,
            SessionExecuteCondition sessionExecuteCondition)
        {
            OwnerIds = new [] { ownerId };
            _createdOn = createdOn;
            _expiryPeriod = expiryPeriod;
            _expiresOn = createdOn + expiryPeriod;
            RunMode = runMode;
            Type = GetType();
            SessionExecuteCondition = sessionExecuteCondition;
        }

        public InteractionSession(
            IEnumerable<ulong> ownerIds,
            DateTime createdOn,
            TimeSpan expiryPeriod,
            RunMode runMode,
            SessionExecuteCondition sessionExecuteCondition)
        {
            OwnerIds = ownerIds;
            _createdOn = createdOn;
            _expiryPeriod = expiryPeriod;
            _expiresOn = createdOn + expiryPeriod;
            RunMode = runMode;
            Type = GetType();
            SessionExecuteCondition = sessionExecuteCondition;
        }

        public bool HasExpired(DateTime currentDate) => _expiresOn <= currentDate;

        public abstract Task<bool> ExecuteAsync(
            SessionContext sessionPayload,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public void ResetExpiry()
        {
            _expiresOn = _expiresOn + _expiryPeriod;
        }

        /// <inheritdoc/>
        public int CompareTo(IInteractionSession? other)
        {
            if (_expiresOn > other.ExpiresOn)
            {
                return 1;
            }

            if (_expiresOn < other.ExpiresOn)
            {
                return -1;
            }

            return 0;
        }

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
