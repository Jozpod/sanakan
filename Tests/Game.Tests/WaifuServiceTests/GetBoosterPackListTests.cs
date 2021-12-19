﻿using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetBoosterPackList(IUser, List{BoosterPack})"/> method.
    /// </summary>
    [TestClass]
    public class GetBoosterPackListTests : Base
    {
        [TestMethod]
        public void Should_Return_Embed()
        {
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var boosterPacks = new List<BoosterPack>
            {
                new BoosterPack
                {
                    CardCount = 5,
                },
            };

            userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            var embed = _waifuService.GetBoosterPackList(userMock.Object, boosterPacks);
            embed.Should().NotBeNull();
        }
    }
}