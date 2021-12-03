using Microsoft.Extensions.DependencyInjection;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sanakan.TaskQueue.MessageHandlers
{
    public static class Extensions
    {
        private static readonly Type _messageHandlerType = typeof(IMessageHandler<>);
        private static IDictionary<Type, Type> _messageHandlerTypeCache = new Dictionary<Type, Type>();

        public static IMessageHandler<T> GetMessageHandler<T>(this IServiceProvider serviceProvider, T message)
           where T : BaseMessage
        {
            var messageType = message.GetType();

            if (!_messageHandlerTypeCache.TryGetValue(messageType, out var messageHandlerType))
            {
                messageHandlerType = _messageHandlerType.MakeGenericType(messageType);
                _messageHandlerTypeCache[messageType] = messageHandlerType;
            }

            return (IMessageHandler<T>)serviceProvider.GetRequiredService(messageHandlerType);
        }
    }
}
