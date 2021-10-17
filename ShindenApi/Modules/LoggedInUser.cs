using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public partial class LoggedInUserModule
    {
        private readonly RequestManager _manager;
        private readonly IInLogger _logger;
        private ILoggedUser _loggedUser;

        public LoggedInUserModule(RequestManager manager, IInLogger logger)
        {
            _logger = logger;
            _manager = manager;
            _loggedUser = null;
        }
    }
}