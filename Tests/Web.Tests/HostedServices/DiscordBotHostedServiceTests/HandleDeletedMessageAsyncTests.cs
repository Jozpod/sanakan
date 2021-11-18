﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class HandleDeletedMessageAsyncTests : Base
    {
        public Cacheable<IMessage, ulong> Create(IMessage value, ulong id)
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]{
                    typeof(IMessage),
                    typeof(ulong),
                    typeof(bool),
                    typeof(Func<Task<IMessage>>),
                };

            var cacheableCtor = typeof(Cacheable<IMessage, ulong>).GetConstructor(
               bindingAttr,
               null, types, null);

            var parameters = new object[] {
                value,
                id,
                true,
                null
            };

            var cacheable = (Cacheable<IMessage, ulong>)cacheableCtor.Invoke(parameters);

            return cacheable;
        }

        [TestMethod]
        public async Task Should_Handle_Deleted_Message()
        {
            await StartAsync();

            var messageMock = new Mock<IMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var socketMessageChannelMock = new Mock<ISocketMessageChannel>(MockBehavior.Strict);

            var userId = 1ul;
            var username = "username";
            var guildId = 1ul;
            var attachments = new Collection<IAttachment>();
            var guildConfig = new GuildOptions(guildId, 50);
            var message = Create(messageMock.Object, 1ul);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object)
                .Verifiable();

            messageMock
                .Setup(pr => pr.CreatedAt)
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();

            messageMock
              .Setup(pr => pr.Attachments)
              .Returns(attachments)
              .Verifiable();

            userMock
                .Setup(pr => pr.Id)
                .Returns(userId)
                .Verifiable();

            userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("avatarUrl")
                .Verifiable();

            userMock
               .Setup(pr => pr.Username)
               .Returns(username)
               .Verifiable();

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false)
                .Verifiable();

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false)
                .Verifiable();

            var guildChannelMock = socketMessageChannelMock.As<IGuildChannel>();

            messageMock
                .Setup(pr => pr.Channel)
                .Returns(socketMessageChannelMock.Object)
                .Verifiable();

            socketMessageChannelMock
                .Setup(pr => pr.Name)
                .Returns("test channel")
                .Verifiable();

            guildChannelMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object)
                .Verifiable();

            guildMock
              .Setup(pr => pr.Id)
              .Returns(guildId)
              .Verifiable();

            var messageChannelMock = socketMessageChannelMock.As<IMessageChannel>();

            guildMock
               .Setup(pr => pr.GetChannelAsync(0, CacheMode.AllowDownload, null))
               .ReturnsAsync(guildChannelMock.Object)
               .Verifiable();

            messageMock
               .Setup(pr => pr.Content)
               .Returns("test message")
               .Verifiable();

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig)
                .Verifiable();

            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object)
                .Verifiable();

            _discordSocketClientAccessorMock.Raise(pr => pr.MessageDeleted += null, message, socketMessageChannelMock.Object);

            guildChannelMock.Verify();
            guildMock.Verify();
            _guildConfigRepositoryMock.Verify();
            messageChannelMock.Verify();
            messageMock.Verify();
            userMock.Verify();
            _taskManagerMock.Verify();
            _discordClientMock.Verify();
        }

    }
}
