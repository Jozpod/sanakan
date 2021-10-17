using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sanakan.ShindenApi;
using Shinden.API;
using Shinden.Extensions;

namespace Sanakan.ShindenApi
{
    public class RequestManager
    {
        private Uri _baseUri;
        private Shinden.Models.ISession _session;
        private readonly CookieContainer _cookies;
        private readonly ILogger _logger; 
        private readonly Auth _auth;

        public RequestManager(Auth auth, ILogger logger)
        {
            _auth = auth;
            _session = null;
            _baseUri = null;
            _logger = logger;
            _cookies = new CookieContainer();
        }

        
    }
}
