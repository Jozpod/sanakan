﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    [TestClass]
    public class ModifyPointsTCDiscordAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Return_Not_Found()
        {

        }

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var discordUserId = 1ul;
            var amount = 100ul;
            var user = new User(discordUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByDiscordIdAsync(discordUserId))
                .ReturnsAsync(user)
                .Verifiable();

            var result = await _controller.ModifyPointsTCDiscordAsync(discordUserId, amount);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();

            _userRepositoryMock.Verify();
        }
    }
}
