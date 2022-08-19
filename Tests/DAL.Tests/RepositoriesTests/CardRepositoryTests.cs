using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class CardRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_Card_By_Id()
        {
            var repository = serviceProvider.GetRequiredService<ICardRepository>();
            var result = await repository.GetByIdAsync(1);
            result.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_Card_By_Character_Id()
        {
            var repository = serviceProvider.GetRequiredService<ICardRepository>();
            var result = await repository.GetByCharacterIdAsync(1);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Card_By_GameDeck_Id()
        {
            var repository = serviceProvider.GetRequiredService<ICardRepository>();
            var result = await repository.GetByGameDeckIdAsync(1);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Card_By_Owner_Id()
        {
            var repository = serviceProvider.GetRequiredService<ICardRepository>();
            var result = await repository.GetByIdFirstOrLastOwnerAsync(1);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
    }
}
