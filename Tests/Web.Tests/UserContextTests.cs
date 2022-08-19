using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;

namespace Sanakan.Web.Test
{
    [TestClass]
    public class UserContextTests
    {
        private readonly IUserContext _userContext;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new(MockBehavior.Strict);
        private readonly Mock<HttpContext> _httpContextMock = new(MockBehavior.Strict);
        private readonly ClaimsPrincipal _claimsPrincipal = new();

        public UserContextTests()
        {
            _httpContextAccessorMock
                .Setup(pr => pr.HttpContext)
                .Returns(_httpContextMock.Object);

            _httpContextMock
                .Setup(pr => pr.User)
                .Returns(_claimsPrincipal);

            _userContext = new UserContext(_httpContextAccessorMock.Object);
        }

        [TestMethod]
        public void Should_Return_Discord_Id()
        {
            var userId = 1ul;
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(nameof(IUserContext.DiscordId), userId.ToString())
            });
            _claimsPrincipal.AddIdentity(claimsIdentity);

            _userContext.DiscordId.Should().NotBeNull();
        }

        [TestMethod]
        public void Should_Return_Null()
        {
            _userContext.DiscordId.Should().BeNull();
        }
    }
}
