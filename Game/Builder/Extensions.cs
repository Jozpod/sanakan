using Microsoft.Extensions.DependencyInjection;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Game.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            services.AddSingleton<IImageProcessor, ImageProcessor>();
            return services;
        }
    }
}
