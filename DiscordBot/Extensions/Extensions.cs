using System;

namespace Sanakan.DiscordBot.Extensions
{
    public static class Extensions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider) => serviceProvider.GetService<T>();
    }
}
