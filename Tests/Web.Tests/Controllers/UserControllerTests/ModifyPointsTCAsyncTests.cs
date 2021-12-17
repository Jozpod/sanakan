using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="UserController.ModifyPointsTCAsync(ulong, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class ModifyPointsTCAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Queue_Action_And_Return_Ok()
        {
            var shindenUserId = 1ul;
            var user = new User(shindenUserId, DateTime.UtcNow);

            _userRepositoryMock
                .Setup(pr => pr.GetByShindenIdAsync(shindenUserId))
                .ReturnsAsync(user);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<TransferTCMessage>()))
                .Returns(true);

            var result = await _controller.ModifyPointsTCAsync(shindenUserId, 100);
            result.Should().BeOfType<ObjectResult>();
        }
    }
}
