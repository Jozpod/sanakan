using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Session
{
    internal class SessionManager : ISessionManager
    {
        private readonly ISet<InteractionSession> _sessions;
        public object SyncRoot { get; } = new object();

        public SessionManager()
        {
            _sessions = new SortedSet<InteractionSession>();
        }

        public bool Exists<T>(ulong discordUserId) where T : InteractionSession
        {
            var exists = false;
            lock (SyncRoot)
            {
                exists = _sessions.Any(pr => pr.OwnerId == discordUserId && pr.Type == typeof(T));
            }
            return exists;
        }

        public void Remove(InteractionSession session)
        {
            lock (SyncRoot)
            {
                _sessions.Remove(session);
            }
        }

        public void Add(InteractionSession session)
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
                var session = _sessions.FirstOrDefault(pr => pr.OwnerId == discordUserId && pr.Type == typeof(T));

                if(session != null)
                {
                    _sessions.Remove(session);
                    session.DisposeAsync();
                }
            }
        }

        public IEnumerable<InteractionSession> GetByOwnerId(ulong OwnerId, SessionExecuteCondition executeCondition)
        {
            lock (SyncRoot)
            {
                var filtered = _sessions.Where(pr => pr.OwnerId == OwnerId
                    && pr.SessionExecuteCondition.HasFlag(executeCondition))
                    .ToList();

                return filtered;
            }
            
        }

        public IEnumerable<InteractionSession> GetExpired(DateTime dateTime)
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
