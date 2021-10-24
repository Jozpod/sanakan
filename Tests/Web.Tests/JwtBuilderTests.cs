using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Api;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Web.Test
{
    [TestClass]
    public class JwtBuilderTests
    {
        private readonly IJwtBuilder _jwtBuilder;
        private readonly Mock<IOptionsMonitor<JwtConfig>> _optionsMock;
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly Mock<ISystemClock> _systemClockMock;

        public JwtBuilderTests()
        {
            _jwtBuilder = new JwtBuilder(
                _optionsMock.Object,
                _encoding,
                _systemClockMock.Object);
        }

        
        [TestMethod]
        public void Should_Generate_Token()
        {
            var tokenData = _jwtBuilder.Build(TimeSpan.FromMinutes(1));
            tokenData.Token.Should().NotBeNullOrEmpty();
        }
    }
}
