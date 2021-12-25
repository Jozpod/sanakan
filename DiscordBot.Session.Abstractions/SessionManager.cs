using System;
using System.Collections.Generic;
using System.Linq;

namespace Sanakan.DiscordBot.Session.Abstractions
{
    internal class SessionManager : ISessionManager
    {
        private readonly ISet<IInteractionSession> _sessions;
        public object SyncRoot { get; } = new object();

        public SessionManager()
        {
            _sessions = new SortedSet<IInteractionSession>();
        }

        public bool Exists<T>(ulong discordUserId)
            where T : IInteractionSession
        {
            var exists = false;
            lock (SyncRoot)
            {
                exists = _sessions
                    .Any(pr => pr.OwnerIds.Contains(discordUserId)
                        && pr.Type == typeof(T));
            }
            return exists;
        }

        public void Remove(IInteractionSession session)
        {
            lock (SyncRoot)
            {
                _sessions.Remove(session);
            }
        }

        public void Add(IInteractionSession session)
        {
            lock (SyncRoot)
            {
                _sessions.Add(session);
            }
        }

        public void RemoveIfExists<T>(ulong discordUserId)
        {
            lock (SyncRoot)
            {
                var session = _sessions
                    .FirstOrDefault(pr => pr.OwnerIds.Contains(discordUserId)
                        && pr.Type == typeof(T));

                if(session != null)
                {
                    _sessions.Remove(session);
                    session.DisposeAsync();
                }
            }
        }

        public IEnumerable<IInteractionSession> GetByOwnerId(ulong OwnerId, SessionExecuteCondition executeCondition)
        {
            lock (SyncRoot)
            {
                var filtered = _sessions.Where(pr => pr.OwnerIds.Contains(OwnerId)
                    && pr.SessionExecuteCondition.HasFlag(executeCondition))
                    .ToList();

                return filtered;
            }
        }

        public IEnumerable<IInteractionSession> GetExpired(DateTime dateTime)
        {
            lock (SyncRoot)
            {
                var filtered = _sessions
                    .Where(pr => pr.HasExpired(dateTime))
                    .ToList();

                return filtered;
            }
        }
    }
}
