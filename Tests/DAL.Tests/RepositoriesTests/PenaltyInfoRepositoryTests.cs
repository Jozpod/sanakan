using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class PenaltyInfoRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_Penalty_Info_By_Guild_Id()
        {
            var repository = ServiceProvider.GetRequiredService<IPenaltyInfoRepository>();

            var result = await repository.GetByGuildIdAsync(1);
            result.First().Should().NotBeNull();
        }

        [TestMethod]
        public async Task Should_Return_Penalty_Info_By_Discord_User()
        {
            var repository = ServiceProvider.GetRequiredService<IPenaltyInfoRepository>();
            var result = await repository.GetPenaltyAsync(1ul, 1ul, PenaltyType.Ban);
            result.Should().NotBeNull();
        }
    }
}
