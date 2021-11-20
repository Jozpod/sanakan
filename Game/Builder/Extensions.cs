using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IImageProcessor, ImageProcessor>();
            services.AddSingleton<IWaifuService, WaifuService>();
            services.AddSingleton<IEventsService, EventsService>();
            services.AddSingleton<SlotMachine>();
            return services;
        }

        public static IServiceCollection AddFontResources(this IServiceCollection services)
        {
            var assembly = typeof(Extensions).Assembly;
            ResourceManager.Add(assembly, Resources.DigitalFont);
            ResourceManager.Add(assembly, Resources.LatoBoldFont);
            ResourceManager.Add(assembly, Resources.LatoLightfont);
            ResourceManager.Add(assembly, Resources.LatoRegularfont);
            return services;
        }
    }
}
