using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using Sanakan.Web.Controllers;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sanakan.Web.Integration.Tests
{
    /// <summary>
    /// Defines tests for <see cref="InfoController"/>.
    /// </summary>
    public partial class TestBase
    {
        [TestMethod]
        public async Task Should_Return_Commands()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            var commands = await _client.GetFromJsonAsync<Commands>("api/info/commands");
            commands.Should().NotBeNull();
            commands.Prefix.Should().Be(".");
            //commands.Modules.Should().HaveCount(6);
            //commands.Modules[0].SubModules[0].Commands.Should().HaveCount(9);
            //commands.Modules[1].SubModules[0].Commands.Should().HaveCount(7);
            //commands.Modules[2].SubModules[0].Commands.Should().HaveCount(3);
            //commands.Modules[3].SubModules[0].Commands.Should().HaveCount(58);
            //commands.Modules[4].SubModules[0].Commands.Should().HaveCount(15);
            //commands.Modules[5].SubModules[0].Commands.Should().HaveCount(6);
        }
    }
}
