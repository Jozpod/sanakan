﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.IntegrationTests
{
    public abstract class TestBase
    {
        protected readonly HttpClient _client;

        public TestBase()
        {
            var factory = new TestWebApplicationFactory();
            _client = factory.CreateClient();
        }
    }
}
