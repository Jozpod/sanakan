using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.TypeReaders;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests
{
    /// <summary>
    /// Defines tests for <see cref="WishlistObjectTypeReader.ReadAsync(ICommandContext, string, IServiceProvider)"/> method.
    /// </summary>
    [TestClass]
    public class WishlistObjectTypeReaderTests
    {
        private readonly WishlistObjectTypeReader _typeReader = new();
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly IServiceProvider _serviceProvider;

        public WishlistObjectTypeReaderTests()
        {
            var serviceCollection = new ServiceCollection();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestMethod]
        [DataRow("c", WishlistObjectType.Card)]
        [DataRow("card", WishlistObjectType.Card)]
        [DataRow("karta", WishlistObjectType.Card)]
        [DataRow("p", WishlistObjectType.Character)]
        [DataRow("postac", WishlistObjectType.Character)]
        [DataRow("postać", WishlistObjectType.Character)]
        [DataRow("character", WishlistObjectType.Character)]
        [DataRow("t", WishlistObjectType.Title)]
        [DataRow("title", WishlistObjectType.Title)]
        [DataRow("tytuł", WishlistObjectType.Title)]
        [DataRow("tytul", WishlistObjectType.Title)]
        public async Task Should_Parse_Values(string input, WishlistObjectType wishlistObjectType)
        {
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.BestMatch.Should().Be(wishlistObjectType);
        }

        [TestMethod]
        public async Task Should_Return_Error()
        {
            var input = "test";
            var result = await _typeReader.ReadAsync(_commandContextMock.Object, input, _serviceProvider);
            result.IsSuccess.Should().BeFalse();
            result.ErrorReason.Should().NotBeNullOrEmpty();
        }
    }
}
