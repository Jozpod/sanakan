using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    public interface ISessionManager
    {
        IReadOnlyCollection<InteractionSession> Sessions { get; }
        void Remove(InteractionSession session);
        void Add(InteractionSession session);
        bool Exists<T>(ulong discordUserId) where T : InteractionSession;
        void RemoveIfExists<T>(ulong discordUserId);
    }
}
