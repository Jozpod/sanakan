using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GetOneFromManyAsync(string[])"/> method.
    /// </summary>
    [TestClass]
    public class GetOneFromManyAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.GetOneFromManyAsync(new[] { "one", "two", "three" });
            _messageChannelMock.Verify();
        }
    }
}
