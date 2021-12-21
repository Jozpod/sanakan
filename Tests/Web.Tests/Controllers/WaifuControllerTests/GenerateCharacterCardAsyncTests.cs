using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GenerateCharacterCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GenerateCharacterCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Enqueue_Task_And_Return_Ok()
        {
            var characterId = 1ul;
            var characterResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterResult);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<UpdateCardPictureMessage>()))
                .Returns(true);
            
            var result = await _controller.GenerateCharacterCardAsync(characterId);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
