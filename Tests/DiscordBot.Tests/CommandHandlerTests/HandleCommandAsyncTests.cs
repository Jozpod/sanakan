using Discord;
using Sanakan.Tests.Shared;
using Discord.Commands;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.CommandHandlerTests
{
    /// <summary>
    /// Defines tests for <see cref="ICommandHandler.HandleCommandAsync(IMessage)"/> method.
    /// </summary>
    [TestClass]
    public class HandleCommandAsyncTests : TestBase
    {
        private readonly Mock<IUserMessage> _userMessageMock = new(MockBehavior.Strict);
        private readonly Mock<IGuildUser> _guildUserMock = new(MockBehavior.Strict);
        private readonly Mock<IMessageChannel> _messageChannelMock = new(MockBehavior.Strict);
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly ulong _userId = 1ul;
        private readonly ulong _guildId = 1ul;

        public HandleCommandAsyncTests()
        {
            _commandHandler.InitializeAsync().GetAwaiter().GetResult();
        }

        public void SetupGuildAndUser()
        {
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);

            _guildUserMock
              .Setup(pr => pr.Id)
              .Returns(_userId);

            _guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            _guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            guildMock
                 .Setup(pr => pr.Id)
                 .Returns(_guildId);
        }

        public void Setup_Process_No_Search_Result(IUserMessage userMessage, Discord.Commands.SearchResult searchResult)
        {
            var channelId = 1ul;
            var guildConfig = new GuildOptions(_guildId, 50);
            _configuration.AllowedToDebug.Add(_userId);

            _userMessageMock
                .Setup(pr => pr.Author)
                .Returns(_guildUserMock.Object);

            _messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_messageChannelMock.Object);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(_guildId))
                .ReturnsAsync(guildConfig);

            _discordClientAccessorMock
                .Setup(pr => pr.GetCommandContext(userMessage))
                .Returns(_commandContextMock.Object);

            _commandContextMock
                .Setup(pr => pr.Message)
                .Returns(userMessage);

            _commandServiceMock
                .Setup(pr => pr.Search(It.IsAny<string>()))
                .Returns(searchResult);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(_messageChannelMock.Object);

            _messageChannelMock
               .Setup(pr => pr.Id)
               .Returns(channelId);
        }

        [TestMethod]
        public void Should_Exit_No_User_Message()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, messageMock.Object);
        }

        [TestMethod]
        public void Should_Exit_Bot()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(true);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Exit_No_Guild_User()
        {
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            userMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Exit_No_String_Prefix()
        {
            var channelId = 1ul;
            var guildId = 1ul;
            var guildConfig = new GuildOptions(guildId, 50);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);

            userMessageMock
                .Setup(pr => pr.Content)
                .Returns("command");

            userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageChannelMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Exit_User_Not_Dev()
        {
            var userId = 1ul;
            var channelId = 1ul;
            var guildId = 1ul;
            var guildConfig = new GuildOptions(guildId, 50);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);

            _userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageChannelMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Process_No_Search_Result_UnknownCommand()
        {
            var searchResult = Discord.Commands.SearchResult.FromError(CommandError.UnknownCommand, "unknown command");
            SetupGuildAndUser();
            Setup_Process_No_Search_Result(
                _userMessageMock.Object,
                searchResult);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(".test");

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Process_No_Search_Result_MultipleMatches()
        {
            var searchResult = Discord.Commands.SearchResult.FromError(CommandError.MultipleMatches, "unknown command");
            SetupGuildAndUser();
            Setup_Process_No_Search_Result(
              _userMessageMock.Object,
              searchResult);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(".test");

            _messageChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Process_No_Search_Result_BadArgCount()
        {
            var searchResult = Discord.Commands.SearchResult.FromError(CommandError.BadArgCount, "bad arg count");
            var commands = new List<CommandMatch>
            {
                new CommandMatch(null, "command"),
            };
            var commandSearchResult = Discord.Commands.SearchResult.FromSuccess("commands", commands);
            SetupGuildAndUser();
            Setup_Process_No_Search_Result(
                _userMessageMock.Object,
                searchResult);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(".test");

            _commandServiceMock
                .Setup(pr => pr.Search(_commandContextMock.Object, 1))
                .Returns(commandSearchResult);

            _messageChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

            _helperServiceMock
                .Setup(pr => pr.GetCommandInfo(It.IsAny<CommandInfo>(), It.IsAny<string?>()))
                .Returns("command info");

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Process_No_Search_Result_UnmetPrecondition_Text()
        {
            var preconditionErrorPayload = new PreconditionErrorPayload();
            preconditionErrorPayload.Message = "unmet precondition";
            var searchResult = Discord.Commands.SearchResult.FromError(CommandError.UnmetPrecondition, preconditionErrorPayload.Serialize());
            SetupGuildAndUser();
            Setup_Process_No_Search_Result(
                _userMessageMock.Object,
                searchResult);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(".test");

            _messageChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Process_No_Search_Result_UnmetPrecondition_Image()
        {
            var preconditionErrorPayload = new PreconditionErrorPayload();
            preconditionErrorPayload.ImageUrl = "http://www.test.com/image.png";
            var searchResult = Discord.Commands.SearchResult.FromError(CommandError.UnmetPrecondition, preconditionErrorPayload.Serialize());
            SetupGuildAndUser();
            Setup_Process_No_Search_Result(
                _userMessageMock.Object,
                searchResult);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(".test");

            _messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>(),
                    It.IsAny<MessageComponent>(),
                    It.IsAny<ISticker[]>(),
                    It.IsAny<Embed[]>()))
                .Callback<Stream, string, string, bool, Embed, RequestOptions, bool, AllowedMentions, MessageReference>(VerifyMessage)
                .ReturnsAsync(_userMessageMock.Object);

            _resourceManagerMock
                .Setup(pr => pr.GetResourceStream(preconditionErrorPayload.ImageUrl))
                .Returns(new MemoryStream());

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);

            void VerifyMessage(
                Stream stream,
                string filename,
                string text,
                bool isTTS,
                Embed embed,
                RequestOptions options,
                bool isSpoiler,
                AllowedMentions allowedMentions,
                MessageReference messageReference)
            {
                stream.Should().NotBeNull();
                embed.Image.Should().NotBeNull();
            }
        }

        private CommandInfo CreateCommand(string moduleName, string commandName, RunMode runMode)
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var commandService = new Discord.Commands.CommandService();
            var moduleBuilder = DiscordInternalExtensions.CreateModuleBuilder(commandService, null);

            moduleBuilder.WithName(moduleName);
            moduleBuilder.Group = ".";
            moduleBuilder.AddCommand(commandName, (cc, objs, sp, ci) => Task.CompletedTask, cb =>
            {
                cb.WithRunMode(runMode);
            });

            var moduleInfo = DiscordInternalExtensions.CreateModuleInfo(moduleBuilder, commandService, serviceProvider);
            return moduleInfo.Commands.First();
        }

        [TestMethod]
        public void Should_Process_Command_Sync()
        {
            var userId = 1ul;
            var channelId = 1ul;
            var guildId = 1ul;
            var guildConfig = new GuildOptions(guildId, 50);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var commands = new List<CommandMatch>
            {
                new CommandMatch(CreateCommand("module", "command", RunMode.Sync), "command"),
            };
            var commandName = "command";
            var messageText = $".{commandName}";
            var commandSearchResult = Discord.Commands.SearchResult.FromSuccess("command", commands);

            _commandServiceMock
                .Setup(pr => pr.Search(commandName))
                .Returns(commandSearchResult);

            _discordClientAccessorMock
                .Setup(pr => pr.GetCommandContext(_userMessageMock.Object))
                .Returns(_commandContextMock.Object);

            _commandContextMock
                .Setup(pr => pr.Message)
                .Returns(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(messageText);

            _userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageChannelMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _commandsAnalyticsRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<CommandsAnalytics>()));

            _commandsAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<CommandMessage>()))
                .Returns(true);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }

        [TestMethod]
        public void Should_Process_Command_Async()
        {
            var userId = 1ul;
            var channelId = 1ul;
            var guildId = 1ul;
            var guildConfig = new GuildOptions(guildId, 50);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var guildMock = new Mock<IGuild>(MockBehavior.Strict);
            var commands = new List<CommandMatch>
            {
                new CommandMatch(CreateCommand("module", "command", RunMode.Async), "command"),
            };
            var commandName = "command";
            var messageText = $".{commandName}";
            var commandSearchResult = Discord.Commands.SearchResult.FromSuccess("command", commands);

            _commandServiceMock
                .Setup(pr => pr.Search(commandName))
                .Returns(commandSearchResult);

            _discordClientAccessorMock
                .Setup(pr => pr.GetCommandContext(_userMessageMock.Object))
                .Returns(_commandContextMock.Object);

            _commandContextMock
                .Setup(pr => pr.Message)
                .Returns(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.Content)
                .Returns(messageText);

            _userMessageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            guildUserMock
                .Setup(pr => pr.IsBot)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.IsWebhook)
                .Returns(false);

            guildUserMock
                .Setup(pr => pr.Guild)
                .Returns(guildMock.Object);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Id)
                .Returns(channelId);

            _discordClientMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(messageChannelMock.Object);

            guildMock
                .Setup(pr => pr.Id)
                .Returns(guildId);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildId))
                .ReturnsAsync(guildConfig);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _commandsAnalyticsRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<CommandsAnalytics>()));

            _commandsAnalyticsRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _discordClientAccessorMock
                .Raise(pr => pr.MessageReceived += null, _userMessageMock.Object);
        }
    }
}