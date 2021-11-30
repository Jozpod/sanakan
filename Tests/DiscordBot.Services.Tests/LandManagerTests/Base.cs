using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ILandManager _landManager;
        protected readonly Mock<IOptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _discordConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new DiscordConfiguration
                {
                    MaxMessageLength = 2000,
                });

            _landManager = new LandManager(_discordConfigurationMock.Object);
        }
    }
}
