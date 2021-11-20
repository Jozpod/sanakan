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
        public async Task Should_Return_GameDeck_By_AnimeId()
        {
            var repository = ServiceProvider.GetRequiredService<IGameDeckRepository>();
            var actual = await repository.GetByAnimeIdAsync(1);
            actual.Should().NotBeNull();
        }
    }
}
