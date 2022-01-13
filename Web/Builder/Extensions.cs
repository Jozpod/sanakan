using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.Api;
using Sanakan.Web;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Sanakan.TaskQueue.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        private const string _pattern = "Sanakan_{0:yyyy}-{0:MM}-{0:dd}.log";
        private static readonly TimeSpan _period = Timeout.InfiniteTimeSpan;
        private static string? _newFileName;
        private static Timer? _timer;

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

        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFile(
                    _pattern,
                    fileLoggerOpts =>
                    {
                        fileLoggerOpts.FileSizeLimitBytes = 1048576;
                        fileLoggerOpts.MinLevel = LogLevel.Debug;

                        var utcNow = DateTime.UtcNow;
                        var dueTime = GetNextDueTime(utcNow);
                        _newFileName = string.Format(_pattern, utcNow);
                        _timer = new Timer(Callback, null, dueTime, _period);

                        fileLoggerOpts.FormatLogFileName = fileName => _newFileName;
                    });
            });

            return services;
        }

        private static TimeSpan GetNextDueTime(DateTime dateTime) => TimeSpan.FromHours(4);

        private static void Callback(object? state)
        {
            var utcNow = DateTime.UtcNow;
            _newFileName = string.Format(_pattern, utcNow);

            var dueTime = GetNextDueTime(utcNow);
            _timer.Change(dueTime, _period);
        }
    }
}
