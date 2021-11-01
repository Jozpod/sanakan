using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    internal class SessionManager : ISessionManager
    {
        private readonly List<InteractionSession> _sessions;

        private readonly object _syncRoot = new object();

        public IReadOnlyCollection<InteractionSession> Sessions => _sessions.AsReadOnly();

        public SessionManager()
        {
            _sessions = new List<InteractionSession>();
        }

        public bool Exists<T>(ulong discordUserId) where T : InteractionSession
        {
            lock (_syncRoot)
            {
                return _sessions.Any(pr => pr.OwnerId == discordUserId && pr.Type == typeof(T));
            }
        }

        public void Remove(InteractionSession session)
        {

            lock (_syncRoot)
            {
                _sessions.Remove(session);
            }
        }

        public void Add(InteractionSession session)
        {
            lock (_syncRoot)
            {
                _sessions.Add(session);
            }
        }

        public void RemoveIfExists<T>(ulong discordUserId)
        {
            lock (_syncRoot)
            {
                var session = _sessions.FirstOrDefault(pr => pr.OwnerId == discordUserId && pr.Type == typeof(T));

                if(session != null)
                {
                    _sessions.Remove(session);
                    session.Dispose();
                }
            }
        }
    }
}
