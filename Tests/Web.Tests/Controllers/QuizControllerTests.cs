using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class QuizControllerTests
    {
        private readonly QuizController _controller;
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly Mock<ICacheManager> _cacheManagerMock;

        public QuizControllerTests()
        {
            _controller = new QuizController(
                _questionRepositoryMock.Object,
                _cacheManagerMock.Object);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
