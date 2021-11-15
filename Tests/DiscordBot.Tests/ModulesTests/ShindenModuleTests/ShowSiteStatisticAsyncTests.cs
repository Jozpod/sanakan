using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Diagnostics;
using System;
using Sanakan.ShindenApi.Models;
using Moq;
using Sanakan.ShindenApi;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models.Enums;
using System.Linq;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    [TestClass]
    public class ShowSiteStatisticAsyncTests : Base
    {
        protected readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Return_Site_Statistics_Image()
        {
            await _module.ShowSiteStatisticAsync(_guildUserMock.Object);
        }
    }
}
