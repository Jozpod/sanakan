using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Sanakan.Services.Executor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Services.Session
{

    public interface ISession
    {
        bool IsValid();
        string GetId();
        IUser GetOwner();
        void MarkAsAdded();
        Task DisposeAsync();
        RunMode GetRunMode();
        ExecuteOn GetEventType();
        bool IsOwner(IUser user);
        void WithLogger(ILogger logger);
        IEnumerable<IUser> GetParticipants();
        bool IsOwner(IEnumerable<IUser> user);
        void SetDestroyer(Func<ISession, Task> destroyer);
        IExecutable GetExecutable(SessionContext context);
    }
}
