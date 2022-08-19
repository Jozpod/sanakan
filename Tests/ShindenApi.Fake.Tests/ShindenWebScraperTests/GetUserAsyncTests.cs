using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetUserAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetUserAsyncTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_User()
        {
            MockHttpOk("Niya (użytkownik) - Shinden.htm");

            var user = await _shindenWebScraper.GetUserAsync(1);
            user.Id.Should().NotBe(0);
            user.Username.Should().NotBeNullOrEmpty();
        }
    }
}