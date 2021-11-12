using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.DiscordBot.Modules;
using Sanakan.TaskQueue;
using Sanakan.ShindenApi;
using Sanakan.Game.Services;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Discord.Commands;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using Sanakan.Game.Services.Abstractions;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    [TestClass]
    public abstract class Base : TestBase
    {
        protected readonly DebugModule _module;
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IBlockingPriorityQueue> _blockingPriorityQueueMock = new(MockBehavior.Strict);
        protected readonly Mock<IWaifuService> _waifuServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IHelperService> _helperServiceMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IWritableOptions<SanakanConfiguration>> _sanakanConfigurationMock = new(MockBehavior.Strict);
        protected readonly Mock<IUserRepository> _userRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<ICardRepository> _cardRepositoryMock = new(MockBehavior.Strict);
        protected readonly Mock<IGuildConfigRepository> _guildConfigRepository = new(MockBehavior.Strict);
        protected readonly Mock<ISystemClock> _systemClockMock = new(MockBehavior.Strict);
        protected readonly Mock<ICacheManager> _cacheManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);
        protected readonly Mock<ITaskManager> _taskManagerMock = new(MockBehavior.Strict);

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

        public Base()
        {
            _module = new(
                _shindenClientMock.Object,
                _blockingPriorityQueueMock.Object,
                _waifuServiceMock.Object,
                _helperServiceMock.Object,
                _imageProcessorMock.Object,
                _sanakanConfigurationMock.Object,
                _userRepositoryMock.Object,
                _cardRepositoryMock.Object,
                _guildConfigRepository.Object,
                _systemClockMock.Object,
                _cacheManagerMock.Object,
                _resourceManagerMock.Object,
                _randomNumberGeneratorMock.Object,
                _taskManagerMock.Object);
            Initialize(_module);
            //var discordSocketClientMock = new Mock<DiscordSocketClient>(MockBehavior.Strict);
            //var socketGlobalUser = CreateSocketGlobalUser(discordSocketClientMock.Object, 1);
            //var socketGuild = CreateSocketGuild(discordSocketClientMock.Object, 1);
            //var socketGuildUser = CreateSocketGuildUser(socketGuild, socketGlobalUser);
        }
    }
}
