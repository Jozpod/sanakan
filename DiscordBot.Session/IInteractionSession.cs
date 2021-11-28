using Discord.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    public interface IInteractionSession : IAsyncDisposable
    {
        ulong OwnerId { get; }

        Type Type { get; }

        RunMode RunMode { get; }

        SessionExecuteCondition SessionExecuteCondition { get; }

        bool IsRunning { get; }

        bool HasExpired(DateTime currentDate);

        Task ExecuteAsync(
            SessionContext sessionPayload,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken = default);

        void ResetExpiry();
    }
}
