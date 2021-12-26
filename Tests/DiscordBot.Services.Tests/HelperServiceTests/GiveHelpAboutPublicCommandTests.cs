using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Tests.Shared;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.GiveHelpAboutPublicCommand(string, string, bool, bool)"/> method.
    /// </summary>
    [TestClass]
    public class GiveHelpAboutPublicCommandTests : Base
    {
        [TestMethod]
        public void Should_Throw_When_Command_Does_Not_Exist()
        {
            var command = "command";
            var prefix = ".";

            Assert.ThrowsException<Exception>(() =>
            {
                _helperService.GiveHelpAboutPublicCommand(command, prefix);
            });
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void Should_Return_Info(bool includePublicModule, bool isAdmin)
        {
            var commandName = "command";
            
            var moderationModuleInfo = DiscordInternalExtensions.CreateModuleWithCommand(PrivateModules.Moderation, "command");
            var debugModuleInfo = DiscordInternalExtensions.CreateModuleWithCommand(PrivateModules.Debug, "command");

            if (includePublicModule)
            {
                var moduleInfo = DiscordInternalExtensions.CreateModuleWithCommand("public-module", commandName);
                _helperService.AddPublicModuleInfo(new[] { moduleInfo });
            }
            
            _helperService.AddPrivateModuleInfo((PrivateModules.Moderation, moderationModuleInfo), (PrivateModules.Debug, debugModuleInfo));
            var result = _helperService.GiveHelpAboutPublicCommand(commandName, string.Empty, isAdmin, true);
            result.Should().NotBeNullOrEmpty();
        }

       
    }
}
