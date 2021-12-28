using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord.Commands;
using Moq;
using Sanakan.Tests.Shared;
using Discord;
using System;
using System.Reflection;
using System.Threading;

namespace Sanakan.DiscordBot.Tests.ModulesTests
{
    /// <summary>
    /// Defines tests for <see cref="SanakanModuleBase"/> class.
    /// </summary>
    [TestClass]
    public class SanakanModuleBaseTests
    {
        private readonly TestModule _module;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<IDisposable> _disposableMock = new(MockBehavior.Strict);

        public class TestModule : SanakanModuleBase
        {
            public override void Dispose() { }
        }

        public SanakanModuleBaseTests()
        {
            _commandContextMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _module = new TestModule();
            DiscordInternalExtensions.SetCommandContext(_module, _commandContextMock.Object);
        }

        public void BeforeExecute()
        {
            var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var afterExecute = _module.GetType().GetMethod(nameof(BeforeExecute), bindingFlags);
            afterExecute.Invoke(_module, new object[] { null });
        }

        [TestMethod]
        public void Should_Set_Typing()
        {
            var manualReset = new ManualResetEvent(false);

            _messageChannelMock
                .Setup(pr => pr.EnterTypingState(null))
                .Returns(_disposableMock.Object);

            _disposableMock
                .Setup(pr => pr.Dispose())
                .Callback(() =>
                {
                    manualReset.Set();
                });

            BeforeExecute();
            manualReset.WaitOne();
        }
    }
}
