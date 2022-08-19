using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class UserRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_User_By_Discord_User_Id()
        {
            var repository = serviceProvider.GetRequiredService<IUserRepository>();
            var result = await repository.GetByDiscordIdAsync(1);
            result.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_User_By_Shinden_User_Id()
        {
            var repository = serviceProvider.GetRequiredService<IUserRepository>();
            var result = await repository.GetByShindenIdAsync(1);
            result.Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_True()
        {
            var repository = serviceProvider.GetRequiredService<IUserRepository>();
            var result = await repository.ExistsByDiscordIdAsync(1);
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_Return_User_By_Shinden_User_Id_Exclude_Discord_User_Id()
        {
            var repository = serviceProvider.GetRequiredService<IUserRepository>();
            var result = await repository.GetByShindenIdExcludeDiscordIdAsync(2, 4);
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }
    }
}
