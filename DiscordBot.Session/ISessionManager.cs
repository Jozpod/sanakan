using System;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Session
{
    public interface ISessionManager
    {
        public object SyncRoot { get; }
        IEnumerable<IInteractionSession> GetByOwnerId(ulong OwnerId, SessionExecuteCondition executeCondition);
        IEnumerable<IInteractionSession> GetExpired(DateTime dateTime);
        void Remove(IInteractionSession session);
        void Add(IInteractionSession session);
        bool Exists<T>(ulong discordUserId) where T : IInteractionSession;
        void RemoveIfExists<T>(ulong discordUserId);
    }
}
