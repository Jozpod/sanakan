using Discord.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    public abstract class InteractionSession : IDisposable
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

        public void ResetExpiry()
        {
            _expiresOn = _expiresOn + _expiryPeriod;
        }

        public virtual void Dispose()
        {
            
        }
    }
}
