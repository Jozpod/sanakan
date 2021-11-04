using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Services;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly HelperService _helperService;
        protected readonly Mock<OptionsMonitor<DiscordConfiguration>> _discordConfigurationMock = new(MockBehavior.Strict);

        public Base()
        {
            _helperService = new(
                _discordConfigurationMock.Object);
        }
    }
}
