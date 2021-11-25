using Discord;
using Discord.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests
{
    [TestClass]
    public abstract class TestBase
    {
        protected readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        protected readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        protected readonly Mock<IUser> _userMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuild> _guildMock = new(MockBehavior.Strict);
        protected readonly Mock<IDiscordClient> _discordClientMock = new(MockBehavior.Strict);
        protected Mock<ITextChannel> _textChannelMock;

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
                .Setup(pr => pr.Client)
                .Returns(_discordClientMock.Object);

            _textChannelMock = _messageChannelMock.As<ITextChannel>();

            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _commandContextMock
                .Setup(pr => pr.User)
                .Returns(_userMock.Object);

            _commandContextMock
                .Setup(pr => pr.Guild)
                .Returns(_guildMock.Object);

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
