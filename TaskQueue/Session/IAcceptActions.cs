using Sanakan.TaskQueue;
using System;
using System.Threading.Tasks;

namespace Sanakan.Services.Session.Models
{
    public interface IAcceptActions
    {
        Task<bool> OnAccept(SessionContext context, IServiceProvider serviceProvider);
        Task<bool> OnDecline(SessionContext context, IServiceProvider serviceProvider);
    }
}
