﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DAL.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddSanakanDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfiguration>(configuration.GetSection("Database"));
            services.AddDbContext<SanakanDbContext>();
            return services;
        }

        public static IServiceCollection AddSanakanDbContextPool(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseConfiguration>(configuration.GetSection("Database"));
            services.AddDbContextPool<SanakanPooledDbContext>((sp, optionsBuilder) =>
            {
                var config = sp.GetRequiredService<IOptionsMonitor<DatabaseConfiguration>>().CurrentValue;

                if (config.Provider == DatabaseProvider.MySql)
                {
                    optionsBuilder.UseMySql(
                        config.ConnectionString,
                        new MySqlServerVersion(config.Version));
                }

                if (config.Provider == DatabaseProvider.Sqlite)
                {
                    optionsBuilder.UseSqlite(config.ConnectionString);
                }

                if (config.Provider == DatabaseProvider.SqlServer)
                {
                    optionsBuilder.UseSqlServer(config.ConnectionString);
                }

                if (config.Provider == DatabaseProvider.InMemory)
                {
                    optionsBuilder.UseInMemoryDatabase("SanakanDb");
                }
            });
            return services;
        }

        public static IServiceCollection AddDatabaseFacade(this IServiceCollection services)
        {
            services.AddSingleton<IDatabaseFacade, DatabaseFacade>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGuildConfigRepository, GuildConfigRepository>();
            services.AddScoped<IGameDeckRepository, GameDeckRepository>();
            services.AddScoped<ITimeStatusRepository, TimeStatusRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IPenaltyInfoRepository, PenaltyInfoRepository>();
            services.AddScoped<ITransferAnalyticsRepository, TransferAnalyticsRepository>();
            services.AddScoped<ISystemAnalyticsRepository, SystemAnalyticsRepository>();
            services.AddScoped<ICommandsAnalyticsRepository, CommandsAnalyticsRepository>();
            services.AddScoped<IUserAnalyticsRepository, UserAnalyticsRepository>();
            return services;
        }
    }
}
