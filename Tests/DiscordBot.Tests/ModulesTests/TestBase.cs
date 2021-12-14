using Discord;
using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System;
using System.Reflection;

namespace DiscordBot.ModulesTests
{
    [TestClass]
    public abstract class TestBase
    {
        protected readonly Mock<IUserMessage> _contextMessageMock = new(MockBehavior.Strict);
        protected readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        protected readonly Mock<ISelfUser> _currentUserMock = new(MockBehavior.Strict);
        protected readonly Mock<IUser> _userMock = new(MockBehavior.Strict);
        protected Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected Mock<ITextChannel> _textChannelMock = new(MockBehavior.Strict);

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

            _discordClientMock
                .Setup(pr => pr.CurrentUser)
                .Returns(_currentUserMock.Object);

            _commandContextMock
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _textChannelMock = _messageChannelMock.As<ITextChannel>();

            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _guildUserMock = _userMock.As<IGuildUser>();

            _commandContextMock
                .Setup(pr => pr.Message)
                .Returns(_contextMessageMock.Object);

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_userMock.Object);

            _commandContextMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);
        }

        protected void SetupSendMessage(Action<string, Embed> action = null)
        {
            Action<string, bool, Embed, RequestOptions, AllowedMentions, MessageReference> defaultAction = (
                text,
                isTTS,
                embed,
                options,
                allowedMentions,
                messageReference) => action?.Invoke(text, embed);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .Callback(defaultAction)
                .ReturnsAsync(_userMessageMock.Object);
        }
    }
}
