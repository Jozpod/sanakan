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
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.ChaosHostedServiceTests
{
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Exit_Not_User_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, messageMock.Object);
        }

        [TestMethod]
        public async Task Should_Exit_Is_Bot()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(true)
                .Verifiable();

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);

            userMock.Verify();
        }

        [TestMethod]
        public async Task Should_Exit_Not_Guild_User()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public async Task Should_Exit_Blacklisted_Guild()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            guildUserMock
               .Setup(pr => pr.IsWebhook)
               .Returns(false)
               .Verifiable();

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            guildMock
                .Setup(pr => pr.Id)
                .Returns(1ul)
                .Verifiable();

            _discordConfiguration.BlacklistedGuilds.Add(1ul);

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);

            guildUserMock.Verify();
        }

        [TestMethod]
        public async Task Should_Swap_User_Nicknames()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>();
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var guildId = 1ul;
            var guildConfig = new GuildOptions(guildId, 50);
            guildConfig.ChaosModeEnabled = true;

            var users = new List<IGuildUser>
            {
                guildUserMock.Object,
                guildUserMock.Object,
            };

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            guildUserMock
               .Setup(pr => pr.IsWebhook)
               .Returns(false)
               .Verifiable();

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            guildUserMock
                .SetupSequence(pr => pr.Id)
                .Returns(1ul)
                .Returns(2ul)
                .Returns(3ul);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId)
                .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig)
                .Verifiable();

            _randomNumberGeneratorMock
                .Setup(pr => pr.TakeATry(3))
                .Returns(true)
                  .Verifiable();

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<IGuildUser>>()))
                .Returns(guildUserMock.Object)
                  .Verifiable();

            guildMock
                .Setup(pr => pr.GetUsersAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(users)
                .Verifiable();

            guildUserMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<GuildUserProperties>>(), null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
            _discordSocketClientAccessorMock.Raise(pr => pr.MessageReceived += null, userMessageMock.Object);

            guildUserMock.Verify();
            guildMock.Verify();
            _randomNumberGeneratorMock.Verify();
            _guildConfigRepositoryMock.Verify();
        }
    }
}
