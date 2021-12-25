using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using System.Threading.Tasks;

namespace Sanakan.Daemon.Tests.HostedServices.DiscordBotHostedServiceTests
{
    [TestClass]
    public class onLogTests : Base
    {
        [TestMethod]
        [DataRow(LogSeverity.Critical)]
        [DataRow(LogSeverity.Debug)]
        [DataRow(LogSeverity.Error)]
        [DataRow(LogSeverity.Info)]
        [DataRow(LogSeverity.Verbose)]
        [DataRow(LogSeverity.Warning)]
        public async Task Should_Log_Message(LogSeverity logSeverity)
        {
            await StartAsync();
            var logMessage = new LogMessage(logSeverity, "source", "message");

            _discordSocketClientAccessorMock.Raise(pr => pr.Log += null, logMessage);
        }

    }
}
