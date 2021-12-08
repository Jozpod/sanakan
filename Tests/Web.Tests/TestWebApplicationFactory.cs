using Discord;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
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

namespace Sanakan.Web.Tests
{
    public class TestWebApplicationFactory : LocalServerFactory<Startup>
    {
        public readonly Mock<IDiscordClientAccessor> DiscordClientAccessorMock = new(MockBehavior.Strict);
        public readonly Mock<IDiscordClient> DiscordClientMock = new(MockBehavior.Strict);
        public readonly Mock<IShindenClient> ShindenClientMock = new(MockBehavior.Strict);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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
                    .Build();

                config.AddConfiguration(configurationBuilder);
            });
        }
    }
}
