using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sanakan.ShindenApi;
using Shinden.API;
using Shinden.Extensions;
using Shinden.Models;

namespace Shinden.Modules
{
    public partial class LoggedInUserModule
    {
        public class UserModule
        {
            private readonly RequestManager _manager;
            private readonly ILogger _logger;

            public LoggedInUserModule OnLoggedIn { get; set; }

            public UserModule(RequestManager manager, ILogger logger)
            {
                _logger = logger;
                _manager = manager;

                OnLoggedIn = new LoggedInUserModule(_manager, _logger);
            }

            
        }
    }
}