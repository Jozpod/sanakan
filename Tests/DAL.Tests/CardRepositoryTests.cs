using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class CardRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<ICardRepository>();
            var entity = new Card(
                1, "test card", "test card",
                10, 20, Rarity.A,
                Dere.Bodere, DateTime.UtcNow);

            repository.Add(entity);
            await repository.SaveChangesAsync();

            var actual = await repository.GetByIdAsync(1);
            actual.Should().BeEquivalentTo(entity);
        }
    }
}
