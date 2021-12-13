using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sanakan.ShindenApi.Tests
{
    [TestClass]
    public class LoginAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_LogIn_And_Put_Cookies()
        {
            MockHttpOk("login-result.json", HttpMethod.Post);

            var expected = new LogInResult
            {
                LoggedUser = new ShindenUser
                {
                    UserId = 1,
                    Id = 1,
                    Avatar = 1,
                    Name = "test",
                    Email = string.Empty,
                    Status = UserStatus.NotSpecified,
                    Sex = Gender.NotSpecified,
                    Rank = string.Empty,
                    PortalLang = Language.NotSpecified,
                    AboutMe = "about me",
                    Signature = "signature",
                    AnimeCss = string.Empty,
                    MangaCss = string.Empty,

                },
                Session = new LogInResultSession
                {
                    Id = "id",
                    Name = "name",
                },
                Hash = "hash",
            };

            var result = await _shindenClient.LoginAsync("username", "password");
            result.Value.Should().BeEquivalentTo(expected);
            var cookies = _cookieContainer.GetCookies(_httpClient.BaseAddress!);
            cookies[0].Name.Should().Be("name");
            cookies[0].Value.Should().Be("name");
            cookies[1].Name.Should().Be("id");
            cookies[1].Value.Should().Be("id");
        }
    }
}
