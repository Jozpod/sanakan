using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Repositories.Abstractions;

using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class GameDeckRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<IGameDeckRepository>();

            var entity = new Models.GameDeck
            {
                Id = 1,
                UserId = 1,
                BackgroundPosition = 10,
                CTCnt = 1,
                DeckPower = 100,
                ForegroundColor = "",
            };

            repository.Add(entity);

            await repository.SaveChangesAsync();

            var actual = await repository.GetByIdAsync(question.Id);
            actual.Should().BeEquivalentTo(question);
            
            repository.Remove(actual);
            await repository.SaveChangesAsync();

            actual = await repository.GetByIdAsync(question.Id);
            actual.Should().BeNull();
        }
    }
}
