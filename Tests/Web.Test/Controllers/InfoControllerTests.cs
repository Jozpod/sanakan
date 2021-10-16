using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class InfoControllerTests
    {
        private readonly InfoController _controller;

        public InfoControllerTests()
        {
            _controller = new InfoController();
        }

        [TestMethod]
        public async Task Should_Retrieve_Commands()
        {

            var commands = await _controller.GetCommandsInfoAsync();
            commands;
        }
    }
}
