﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Web.Controllers;
using System;

namespace Sanakan.Web.Tests.Controllers.QuizControllerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly QuizController _controller;
        protected readonly Mock<IQuestionRepository> _questionRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);

        public Base()
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