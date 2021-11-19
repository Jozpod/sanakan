using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public abstract class InteractionSession : IComparable<InteractionSession>, IAsyncDisposable
    {
        public ulong OwnerId { get; }
        private readonly DateTime _createdOn;
        private readonly TimeSpan _expiryPeriod;
        private DateTime _expiresOn;

        public Type Type { get; }
        public RunMode RunMode { get; }
        public SessionExecuteCondition SessionExecuteCondition { get; }

        public InteractionSession(
            ulong ownerId,
            DateTime createdOn,
            TimeSpan expiryPeriod,
            RunMode runMode,
            SessionExecuteCondition sessionExecuteCondition)
        {
            OwnerId = ownerId;
            _createdOn = createdOn;
            _expiryPeriod = expiryPeriod;
            _expiresOn = createdOn + expiryPeriod;
            RunMode = runMode;
            Type = GetType();
            SessionExecuteCondition = sessionExecuteCondition;
        }

        public bool HasExpired(DateTime currentDate) => _expiresOn <= currentDate;

        public abstract Task ExecuteAsync(
            SessionContext sessionPayload,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public void ResetExpiry()
        {
            _expiresOn = _expiresOn + _expiryPeriod;
        }

        /// <inheritdoc/>
        public int CompareTo(InteractionSession? other)
        {
            if (_expiresOn > other._expiresOn)
            {
                return 1;
            }

            if (_expiresOn < other._expiresOn)
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
