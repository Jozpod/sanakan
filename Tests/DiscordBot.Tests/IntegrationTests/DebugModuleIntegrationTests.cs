using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DiscordBot.Builder;
using Sanakan.DiscordBot.Services.Builder;
using Sanakan.DiscordBot.Session.Builder;
using Sanakan.Game.Builder;
using Sanakan.ShindenApi.Builder;
using Sanakan.TaskQueue.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    public partial class TestBase
    {
        [TestMethod]
        public async Task Should_Run()
        {
            var guild = await _client.GetGuildAsync(_configuration.MainGuild);
            var mainChannel = 910284207534796800ul;

            var textChannel = await guild.GetTextChannelAsync(mainChannel);

            var message = ".ktoto <@!363676687667429376>";
            await textChannel.SendMessageAsync(".ktoto @Jotpe#5246");
            //await textChannel.SendMessageAsync(".info");

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
