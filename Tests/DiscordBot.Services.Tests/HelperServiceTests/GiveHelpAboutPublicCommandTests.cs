using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    [TestClass]
    public class GiveHelpAboutPublicCommandTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Success()
        {
            var command = "command";
            var prefix = ".";

            var result = _helperService.GiveHelpAboutPublicCommand(command, prefix);
            result.Should().NotBeEmpty();
        }
    }
}
