using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Tests.Shared;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GivePublicHelp"/> method.
    /// </summary>
    [TestClass]
    public class GivePublicHelpTests : Base
    {
        [TestMethod]
        public void Should_Return_Info()
        {
            var moduleInfo = DiscordInternalExtensions.CreateModuleWithCommand("public-module", "command");

            _helperService.AddPublicModuleInfo(new[] { moduleInfo });
            var result = _helperService.GivePublicHelp();
            result.Should().NotBeNullOrEmpty();
        }
    }
}
