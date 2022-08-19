using Discord;
using Moq;
using System.IO;

namespace Sanakan.Tests.Shared
{
    public static class IMessageChannelExtensions
    {
        public static void SetupSendMessageAsync(
            this Mock<IDMChannel> dmChannelMock,
            IUserMessage? userMessage = null)
        {
            dmChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>(),
                    It.IsAny<MessageComponent>(),
                    It.IsAny<ISticker[]>(),
                    It.IsAny<Embed[]>()))
                .ReturnsAsync(userMessage);
            }

        public static void SetupSendMessageAsync(
            this Mock<ITextChannel> textChannelMock,
            IUserMessage? userMessage = null)
        {
            textChannelMock
            .Setup(pr => pr.SendMessageAsync(
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<Embed>(),
                It.IsAny<RequestOptions>(),
                It.IsAny<AllowedMentions>(),
                It.IsAny<MessageReference>(),
                It.IsAny<MessageComponent>(),
                It.IsAny<ISticker[]>(),
                It.IsAny<Embed[]>()))
            .ReturnsAsync(userMessage);
        }

        public static void SetupSendFileAsync(
          this Mock<ITextChannel> messageChannelMock,
          IUserMessage? userMessage = null)
        {
            messageChannelMock
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
                .ReturnsAsync(userMessage);
        }

        public static void SetupSendMessageAsync(
            this Mock<IMessageChannel> messageChannelMock,
            IUserMessage? userMessage = null)
        {
            messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>(),
                    It.IsAny<MessageComponent>(),
                    It.IsAny<ISticker[]>(),
                    It.IsAny<Embed[]>()))
                .ReturnsAsync(userMessage);
        }

        public static void SetupSendFileAsync(
            this Mock<IMessageChannel> messageChannelMock,
            IUserMessage? userMessage = null)
        {
            messageChannelMock
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
                .ReturnsAsync(userMessage);
        }
    }
}
