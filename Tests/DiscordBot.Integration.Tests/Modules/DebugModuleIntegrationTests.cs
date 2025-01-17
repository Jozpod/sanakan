﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Abstractions;
using Sanakan.DiscordBot.Integration.Tests.CommandBuilders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Integration.Tests
{
#if DEBUG
    [TestClass]
#endif
    public class DebugModuleIntegrationTests : TestBase
    {
        [TestMethod]
        public async Task TC701_Should_Change_User_Ac()
        {
            var commandMessage = DebugCommandBuilder.ChangeUserAc(prefix, FakeUser.Mention, 1000);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC702_Should_Change_User_Ct()
        {
            var commandMessage = DebugCommandBuilder.ChangeUserCt(prefix, FakeUser.Mention, 1000);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC703_Should_Change_User_Level()
        {
            var commandMessage = DebugCommandBuilder.ChangeUserLevel(prefix, FakeUser.Mention, 1000);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC704_Should_Change_Card_Title()
        {
            var commandMessage = DebugCommandBuilder.ChangeTitleCard(prefix, 1, "new title");
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC705_Should_Give_Away_Card()
        {
            var commandMessage = DebugCommandBuilder.GiveawayCards(prefix, 1, 2, TimeSpan.FromSeconds(10));
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();

            await message.AddReactionAsync(Emojis.Checked);

            message = await WaitForMessageAsync();
            message.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC706_Should_Give_Away_Cards()
        {
            var commandMessage = DebugCommandBuilder.GiveawayCardsMulti(prefix, 1, 2, TimeSpan.FromSeconds(30), 2);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
