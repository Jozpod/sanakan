using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    internal class SessionManager : ISessionManager
    {
        public Task KillSessionIfExistAsync(object session)
        {
            throw new NotImplementedException();
        }

        public bool SessionExist(object session)
        {
            throw new NotImplementedException();
        }

        public Task TryAddSession(object session)
        {
            throw new NotImplementedException();
        }
    }
}
