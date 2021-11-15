using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Models;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    [TestClass]
    public class GiveUserAPacksAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var discordUserId = 1ul;
            var cardBoosterPacks = new[]
            {
                new CardBoosterPack()
            };

            var result = await _controller.GiveUserAPacksAsync(discordUserId, cardBoosterPacks);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
