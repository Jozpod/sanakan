using Microsoft.Extensions.DependencyInjection;
using Sanakan.Api;
using Sanakan.Web;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.TaskQueue.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddJwtBuilder(this IServiceCollection services)
        {
            services.AddSingleton<IJwtBuilder, JwtBuilder>();
            return services;
        }

        public static IServiceCollection AddRequestBodyReader(this IServiceCollection services)
        {
            services.AddTransient<IRequestBodyReader, RequestBodyReader>();
            return services;
        }

        public static IServiceCollection AddUserContext(this IServiceCollection services)
        {
            services.AddTransient<IUserContext, UserContext>();
            return services;
        }
    }
}
