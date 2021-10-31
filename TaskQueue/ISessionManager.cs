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
        Task KillSessionIfExistAsync(object session);
        bool SessionExist(object session);
        Task TryAddSession(object session);
    }
}
