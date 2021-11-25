using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using Sanakan.Common.Converters;
using Sanakan.DAL;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.IntegrationTests
{
    [TestClass]
    public partial class TestBase
    {
        protected static HttpClient _client;
        protected static IDatabaseFacade _databaseFacade;
        protected static readonly JsonSerializerOptions _jsonSerializerOptions;

        static TestBase()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new TimeSpanConverter());
        }

        public static async Task AuthorizeAsync()
        {
            var apiKey = "test key";
            var response = await _client.PostAsJsonAsync("api/token", apiKey);
            var tokenData = await response.Content.ReadFromJsonAsync<TokenData>();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenData.Token);
        }

        [ClassInitialize]
        public static async Task Setup(TestContext context)
        {
            var factory = new TestWebApplicationFactory();
            _client = factory.CreateClient();
            var serviceScope = factory.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            _databaseFacade = serviceProvider.GetRequiredService<IDatabaseFacade>();
            var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();

            await _databaseFacade.EnsureCreatedAsync();

            var testUser = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);

            dbContext.Users.Add(testUser);
            await dbContext.SaveChangesAsync();

            testUser.GameDeck.Cards.Add(card);
            await dbContext.SaveChangesAsync();

            await AuthorizeAsync();
        }

        [ClassCleanup]
        public static async Task Cleanup()
        {
            await _databaseFacade.EnsureDeletedAsync();
            _client.Dispose();
        }
    }
}