using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
        public async Task Should_Return_Card_By_Id()
        {
            var repository = ServiceProvider.GetRequiredService<ICardRepository>();
            var actual = await repository.GetByIdAsync(1);
            actual.Should().NotBeNull();
        }
    }
}
