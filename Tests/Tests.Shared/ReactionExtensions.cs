using Discord;
using Discord.WebSocket;
using Moq;
using System.Reflection;

namespace Sanakan.Tests.Shared
{
    public static class ReactionExtensions
    {
        public static IReaction CreateReaction(
           ulong userId,
           IEmote emote)
        {
            var messageChannelMock = new Mock<ISocketMessageChannel>();

            return CreateReaction(
                messageChannelMock.Object,
                1ul,
                Optional<SocketUserMessage>.Unspecified,
                userId,
                Optional<IUser>.Unspecified,
                emote);
        }

        public static IReaction CreateReaction(
            ISocketMessageChannel messageChannel,
            ulong messageId,
            Optional<SocketUserMessage> userMessage,
            ulong userId,
            Optional<IUser> user,
            IEmote emote)
        {
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            var types = new[]
            {
                typeof(ISocketMessageChannel),
                typeof(ulong),
                typeof(Optional<SocketUserMessage>),
                typeof(ulong),
                typeof(Optional<IUser>),
                typeof(IEmote),
            };
            var socketReactionCtor = typeof(SocketReaction).GetConstructor(bindingAttr, null, types, null);

            var parameters = new object[]
            {
                messageChannel,
                messageId,
                userMessage,
                userId,
                user,
                emote
            };

            var reaction = (IReaction)socketReactionCtor.Invoke(parameters);

            return reaction;
        }
    }
}
