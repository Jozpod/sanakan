using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using FluentAssertions;
using Sanakan.DiscordBot.Services.Abstractions;
using System;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GiveHelpAboutPublicCommand(string, string, bool, bool)"/> method.
    /// </summary>
    [TestClass]
    public class GiveHelpAboutPublicCommandTests : Base
    {
        [TestMethod]
        public async Task Should_Throw_When_Command_Does_Not_Exist()
        {
            var command = "command";
            var prefix = ".";

            Assert.ThrowsException<Exception>(() =>
            {
                _helperService.GiveHelpAboutPublicCommand(command, prefix);
            });
        }
    }
}
