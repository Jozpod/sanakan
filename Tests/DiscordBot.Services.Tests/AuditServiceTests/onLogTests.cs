using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DiscordBot.ServicesTests.AuditServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="AuditService.onLog"/> event handler.
    /// </summary>
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
        public void Should_Log_Message(LogSeverity logSeverity)
        {
            var logMessage = new LogMessage(logSeverity, "source", "message");

            _discordClientAccessorMock.Raise(pr => pr.Log += null, logMessage);
        }

    }
}
