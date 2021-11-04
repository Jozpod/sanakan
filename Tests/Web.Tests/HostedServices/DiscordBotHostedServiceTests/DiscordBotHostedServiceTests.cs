using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Web.HostedService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Tests.HostedServices;

namespace Sanakan.Web.Test.HostedServices
{
    [TestClass]
    public class MemoryUsageHostedServiceTests
    {

        public MemoryUsageHostedServiceTests()
        {
         
        }



        [TestMethod]
        public async Task Should_Save_Memory_Usage_On_Tick()
        {

        }
    }
}
