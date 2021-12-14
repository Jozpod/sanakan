using Discord;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Sanakan.DiscordBot;
using Sanakan.ShindenApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Integration.Tests
{
    [ExcludeFromCodeCoverage]
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
         where TStartup : class
    {
        private const string _LocalhostBaseAddress = "https://localhost";
        private IWebHost _host = null!;
        public readonly Mock<IDiscordClientAccessor> DiscordClientAccessorMock = new(MockBehavior.Strict);
        public readonly Mock<IDiscordClient> DiscordClientMock = new(MockBehavior.Strict);
        public readonly Mock<IShindenClient> ShindenClientMock = new(MockBehavior.Strict);
        public string RootUri { get; private set; } = null!;

        public TestWebApplicationFactory(bool useServer = false)
        {
            ClientOptions.BaseAddress = new Uri(_LocalhostBaseAddress);
            // Breaking change while migrating from 2.2 to 3.1, TestServer was not called anymore

            if (useServer)
            {
                CreateServer(CreateWebHostBuilder());
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.UseSolutionRelativeContentRoot("Web");

            builder.ConfigureTestServices(services =>
            {
                services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
            });

            builder.ConfigureTestServices(services =>
            {
                DiscordClientAccessorMock
                    .Setup(pr => pr.Client)
                    .Returns(DiscordClientMock.Object);

                DiscordClientAccessorMock
                    .Setup(pr => pr.LoginAsync(TokenType.Bot, It.IsAny<string>(), true))
                    .Returns(Task.CompletedTask);

                services.AddSingleton(DiscordClientAccessorMock.Object);
                services.AddSingleton(ShindenClientMock.Object);
            });

            builder.ConfigureAppConfiguration(config =>
            {
                var configurationBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                config.AddConfiguration(configurationBuilder);
            });
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            _host = builder.Build();
            _host.Start();
            RootUri = _host.ServerFeatures.Get<IServerAddressesFeature>()
                .Addresses
                .LastOrDefault()!;

            var serverBuilder = new WebHostBuilder();

            ConfigureWebHost(serverBuilder);
            serverBuilder.UseStartup<TestServerStartup>();

            return new TestServer(serverBuilder);
        }
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            ConfigureWebHost(builder);
            builder.UseStartup<TStartup>();
            return builder;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _host?.Dispose();
            }
        }
    }
}
