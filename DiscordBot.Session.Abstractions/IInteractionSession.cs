using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session.Abstractions
{
    public interface IInteractionSession : IAsyncDisposable, IComparable<IInteractionSession>
    {
        IEnumerable<ulong> OwnerIds { get; }

        Type Type { get; }

        RunMode RunMode { get; }

        SessionExecuteCondition SessionExecuteCondition { get; }

        bool IsRunning { get; }

        DateTime ExpiresOn { get; }

        IServiceProvider ServiceProvider { get; set; }

        bool HasExpired(DateTime currentDate);

        Task<bool> ExecuteAsync(
            SessionContext sessionPayload,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default);

        void ResetExpiry();
    }
}
