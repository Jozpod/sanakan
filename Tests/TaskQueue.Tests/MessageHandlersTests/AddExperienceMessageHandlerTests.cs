using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class AddExperienceMessageHandlerTests
    {
        private readonly AddExperienceMessageHandler _messageHandler;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        private readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        private readonly Mock<IUserAnalyticsRepository> _userAnalyticsRepositoryMock = new(MockBehavior.Strict);

        public AddExperienceMessageHandlerTests()
        {
            _messageHandler = new(
                _systemClockMock.Object,
                _userRepositoryMock.Object,
                _guildConfigRepositoryMock.Object,
                _userAnalyticsRepositoryMock.Object);
        }

        [TestMethod]
        public async Task Should_Add_Message()
        {
            var message = new AddExperienceMessage()
            {

            };

            await _messageHandler.HandleAsync(message);
        }
    }
}
