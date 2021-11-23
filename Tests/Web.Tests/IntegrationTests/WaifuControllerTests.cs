using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Sanakan.Web.Controllers;
using System.Threading.Tasks;
using static Sanakan.Web.ResponseExtensions;
using Sanakan.DAL.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using Sanakan.Common.Converters;

namespace Sanakan.Web.Tests.IntegrationTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController"/>.
    /// </summary>
    [TestClass]
    public class WaifuControllerTests : TestBase
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions;

        static WaifuControllerTests()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
        }

        [TestMethod]
        public async Task Should_Return_Commands()
        {
            var commands = await _client.GetFromJsonAsync<Commands>("api/info/commands");
            commands.Should().NotBeNull();
        }
    }
}
