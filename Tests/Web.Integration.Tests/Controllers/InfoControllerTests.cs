﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Web.Controllers;
using Sanakan.Web.Models;
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
        }
    }
}
