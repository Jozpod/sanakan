using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Api.Models;
using Sanakan.Common.Converters;
using Sanakan.DAL;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sanakan.Web.Integration.Tests
{
#if DEBUG
    [TestClass]
#endif
    public partial class TestBase
    {
        protected static TestWebApplicationFactory<Startup> _factory = null!;
        protected static HttpClient _client = null!;
        protected static IDatabaseFacade _databaseFacade = null!;
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
            _factory = new TestWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            var serviceScope = _factory.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;
            _databaseFacade = serviceProvider.GetRequiredService<IDatabaseFacade>();
            var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();

            var created = await _databaseFacade.EnsureCreatedAsync();

            if (created)
            {
                await TestDataGenerator.RunAsync(dbContext);
            }

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