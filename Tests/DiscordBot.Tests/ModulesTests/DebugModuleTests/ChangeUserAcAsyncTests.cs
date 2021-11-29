using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.Configuration;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ChangeUserAcAsync(IGuildUser, long)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeUserAcAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            await _module.ChangeUserAcAsync(null, 0);
        }
    }
}
