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
    /// Defines tests for <see cref="ShindenWebScraper.GetAnimeDetailsAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class GetAnimeDetailsAsyncTests : ShindenWebScraperTestBase
    {
        [TestMethod]
        public async Task Should_Return_Anime_List()
        {
            MockHttpOk("Lista Anime - Shinden.htm");

            var details = await _shindenWebScraper.GetAnimeDetailsAsync(1);
            details.Should().NotBeEmpty();
            var item = details.First();
            item.Id.Should().NotBe(0);
            item.Name.Should().NotBeNullOrEmpty();
        }
    }
}
