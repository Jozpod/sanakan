using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class UserRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_User_By_Discord_User_Id()
        {
            var repository = ServiceProvider.GetRequiredService<IUserRepository>();
            var actual = await repository.GetByDiscordIdAsync(1);
            actual.Should().NotBeNull();
        }
    }
}
