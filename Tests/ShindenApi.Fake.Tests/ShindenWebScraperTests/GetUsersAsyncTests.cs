using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Fake.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenWebScraper.GetUsersAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class GetUsersAsyncTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Users()
        {
            MockHttpOk("Lista użytkowników - Shinden.htm");

            var users = await _shindenWebScraper.GetUsersAsync("test");
            users.Should().NotBeEmpty();
            var user = users.First();
            user.Id.Should().NotBe(0);
            user.Username.Should().NotBeNullOrEmpty();
        }
    }
}
