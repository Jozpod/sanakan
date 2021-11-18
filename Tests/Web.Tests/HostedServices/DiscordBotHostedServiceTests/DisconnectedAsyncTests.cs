using Discord;
using Discord.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class DisconnectedAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Try_Reconnect()
        {
            await StartAsync();
            var exception = new Exception("test");

            _taskManagerMock
                .Setup(pr => pr.Delay(
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.ConnectionState)
                .Returns(ConnectionState.Disconnected)
                .Verifiable();

            _discordClientMock
                .Setup(pr => pr.StartAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordSocketClientAccessorMock.Raise(pr => pr.Disconnected += null, exception);

            _taskManagerMock.Verify();
            _discordClientMock.Verify();
        }

    }
}
