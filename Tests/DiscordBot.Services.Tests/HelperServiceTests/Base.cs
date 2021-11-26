using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Services;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly IHelperService _helperService;
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {

                });

            _helperService = new HelperService(
                _discordConfigurationMock.Object);
        }
    }
}
