using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common.Builder;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Game.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IImageProcessor, ImageProcessor>();
            services.AddSingleton<IWaifuService, WaifuService>();
            services.AddSingleton<IEventsService, EventsService>();
            services.AddSingleton<ISlotMachine, SlotMachine>();
            return services;
        }

        public static IServiceCollection AddImageResolver(this IServiceCollection services)
        {
            services.AddSingleton<IImageResolver, ImageResolver>();
            return services;
        }

        public static IServiceCollection AddFakeImageResolver(this IServiceCollection services)
        {
            services.AddSingleton<IImageResolver, FakeImageResolver>();
            return services;
        }

        public static ResourceManagerBuilder AddFontResources(this ResourceManagerBuilder builder)
        {
            var assembly = typeof(Extensions).Assembly;
            builder.AssemblyResourceMap.Add(Resources.DigitalFont, assembly);
            builder.AssemblyResourceMap.Add(Resources.LatoBoldFont, assembly);
            builder.AssemblyResourceMap.Add(Resources.LatoLightfont, assembly);
            builder.AssemblyResourceMap.Add(Resources.LatoRegularfont, assembly);
            return builder;
        }
    }
}
