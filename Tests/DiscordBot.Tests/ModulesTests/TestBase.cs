using Discord;
using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.Tests.Shared;
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
        protected readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        protected Mock<ITextChannel> _textChannelMock = new(MockBehavior.Strict);

        protected void Initialize(SanakanModuleBase moduleBase)
        {
            DiscordInternalExtensions.SetCommandContext(moduleBase, _commandContextMock.Object);

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
            Action<string,
                bool,
                Embed,
                RequestOptions,
                AllowedMentions,
                MessageReference,
                MessageComponent,
                ISticker[],
                Embed[]> defaultAction = (
                text,
                isTTS,
                embed,
                options,
                allowedMentions,
                messageReference,
                messageComponent,
                stickers,
                embeds) => action?.Invoke(text, embed);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>(),
                    It.IsAny<MessageComponent>(),
                    It.IsAny<ISticker[]>(),
                    It.IsAny<Embed[]>()))
                .Callback(defaultAction)
                .ReturnsAsync(_userMessageMock.Object);
        }
    }
}
