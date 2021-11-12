using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.DiscordBot.Modules;
using Sanakan.Common;

namespace DiscordBot.ModulesTests.LandsModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly LandsModule _module;
        protected readonly Mock<ILandManager> _landManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

        public Base()
        {
            _module = new(
                _landManagerMock.Object,
                _guildConfigRepositoryMock.Object,
                _taskManagerMock.Object);
            Initialize(_module);
        }

        //public class FakeSocketCommandContext : SocketCommandContext
        //{
        //    private readonly SocketGuild _guild;

        //    public FakeSocketCommandContext(DiscordSocketClient client, SocketUserMessage msg, SocketGuild socketGuild) : base(client, msg)
        //    {
        //        _guild = socketGuild;
        //    }
        //}

        //public static object CreateSocketGlobalUser(DiscordSocketClient discordSocketClient, ulong id)
        //{
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //    var discordNetAssembly = assemblies
        //        .FirstOrDefault(pr => pr.FullName == "Discord.Net.WebSocket, Version=2.4.0.0, Culture=neutral, PublicKeyToken=null");

        //    var socketGlobalUserType = discordNetAssembly.GetType("Discord.WebSocket.SocketGlobalUser");


        //    var socketGlobalUserCtor = socketGlobalUserType.GetConstructor(
        //        BindingFlags.NonPublic | BindingFlags.Instance,
        //        null, new[]{
        //                typeof(DiscordSocketClient),
        //                typeof(ulong),
        //            }, null);

        //    var parameters = new object[] {
        //        discordSocketClient, id
        //    };

        //    var socketGlobalUser = socketGlobalUserCtor.Invoke(parameters);
        //    return socketGlobalUser;
        //}

        //public static SocketGuildUser CreateSocketGuildUser(SocketGuild socketGuild, object socketGlobalUser)
        //{
        //    var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        //    var types = new[]{
        //        typeof(SocketGuild),
        //        socketGlobalUser.GetType(),
        //    };
        //    var socketGuildUserCtor = typeof(SocketGuildUser).GetConstructor(
        //       bindingAttr,
        //       null, types, null);

        //    var parameters = new object[] {
        //        socketGuild, socketGlobalUser
        //    };

        //    var socketGuildUser = (SocketGuildUser)socketGuildUserCtor.Invoke(parameters);

        //    return socketGuildUser;
        //}

        //public static SocketGuildChannel CreateSocketGuildChannel(
        //    DiscordSocketClient discordSocketClient, ulong id, SocketGuild socketGuild)
        //{
        //    var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        //    var types = new[]{
        //        typeof(DiscordSocketClient),
        //        typeof(ulong),
        //        typeof(SocketGuild),
        //    };
        //    var socketGuildChannelCtor = typeof(SocketGuildChannel)
        //        .GetConstructor(bindingAttr, null, types, null);

        //    var parameters = new object[] {
        //        discordSocketClient,
        //        id,
        //        socketGuild
        //    };

        //    var socketGuildChannel = (SocketGuildChannel)socketGuildChannelCtor.Invoke(parameters);

        //    return socketGuildChannel;
        //}

        //public static SocketGuild CreateSocketGuild(DiscordSocketClient discordSocketClient, ulong id)
        //{
        //    var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        //    var socketGuildCtor = typeof(SocketGuild).GetConstructor(
        //     bindingAttr,
        //     null, new[]{
        //            typeof(DiscordSocketClient),
        //            typeof(ulong),
        //     }, null);

        //    var socketGuild = (SocketGuild)socketGuildCtor.Invoke(new object[] {
        //        discordSocketClient, id,
        //    });
        //    return socketGuild;
        //}

        //public static SocketUserMessage CreateSocketUserMessage(
        //    DiscordSocketClient discordSocketClient,
        //    ulong id,
        //    ISocketMessageChannel socketMessageChannel,
        //    SocketGuildUser socketGuildUser)
        //{
        //    var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
        //    var types = new[]{
        //        typeof(DiscordSocketClient),
        //        typeof(ulong),
        //        typeof(ISocketMessageChannel),
        //        typeof(SocketUser),
        //        typeof(MessageSource),
        //    };
        //    var socketUserMessageCtor = typeof(SocketUserMessage)
        //        .GetConstructor(bindingAttr, null, types, null);

        //    var socketUserMessage = (SocketUserMessage)socketUserMessageCtor.Invoke(new object[] {
        //        discordSocketClient,
        //        id,
        //        socketMessageChannel,
        //        socketGuildUser,
        //        MessageSource.User,
        //    });

        //    return socketUserMessage;
        //}
    }
}
