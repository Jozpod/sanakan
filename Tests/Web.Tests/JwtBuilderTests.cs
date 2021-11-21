using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Api;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using System;
using System.Text;

namespace Sanakan.Web.Test
{
    [TestClass]
    public class JwtBuilderTests
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly Mock<IOptionsMonitor<JwtConfiguration>> _optionsMock = new(MockBehavior.Strict);
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);

        public JwtBuilderTests()
        {
            var options = new JwtConfiguration
            {
                IssuerSigningKey = "qazxswedcvfrtgb1",
                Issuer = "test",
            };

            _optionsMock
                .Setup(pr => pr.CurrentValue)
                .Returns(options)
                .Verifiable();

            _jwtBuilder = new JwtBuilder(
                _optionsMock.Object,
                _encoding,
                _systemClockMock.Object);
        }

        
        [TestMethod]
        public void Should_Generate_Token()
        {
            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow)
                .Verifiable();

            var tokenData = _jwtBuilder.Build(TimeSpan.FromMinutes(1));
            tokenData.Token.Should().NotBeNullOrEmpty();

            _optionsMock.Verify();
            _systemClockMock.Verify();
        }
    }
}
