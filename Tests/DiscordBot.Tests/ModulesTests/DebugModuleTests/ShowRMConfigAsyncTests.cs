using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;
using Sanakan.DAL.Models;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ShowRMConfigAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowRMConfigAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_RM_Config()
        {
         
            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ShowRMConfigAsync();
        }
    }
}
