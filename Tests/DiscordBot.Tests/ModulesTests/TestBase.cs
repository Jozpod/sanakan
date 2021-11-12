using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.TaskQueue;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.ModulesTests
{
    [TestClass]
    public abstract class TestBase
    {
        protected readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        protected readonly Mock<IUser> _userMock = new(MockBehavior.Strict);

        protected void SetContext(SanakanModuleBase moduleBase)
        {
            var setContext = moduleBase.GetType().GetMethod(
             "Discord.Commands.IModuleBase.SetContext",
             BindingFlags.NonPublic | BindingFlags.Instance);
            setContext.Invoke(moduleBase, new object[] { _commandContextMock.Object });
        }

        protected void Initialize(SanakanModuleBase moduleBase)
        {
            SetContext(moduleBase);

            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_userMock.Object);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);
        }
    }
}
