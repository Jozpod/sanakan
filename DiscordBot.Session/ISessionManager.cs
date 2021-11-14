using System;
using System.Collections.Generic;

namespace Sanakan.DiscordBot.Session
{
    public interface ISessionManager
    {
        public object SyncRoot { get; }
        IEnumerable<InteractionSession> GetByOwnerId(ulong OwnerId, SessionExecuteCondition executeCondition);
        IEnumerable<InteractionSession> GetExpired(DateTime dateTime);
        void Remove(InteractionSession session);
        void Add(InteractionSession session);
        bool Exists<T>(ulong discordUserId) where T : InteractionSession;
        void RemoveIfExists<T>(ulong discordUserId);
    }
}
