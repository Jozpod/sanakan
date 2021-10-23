using Microsoft.Extensions.DependencyInjection;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Builder
{
    public static class Extensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGuildConfigRepository, GuildConfigRepository>();
            services.AddScoped<IGameDeckRepository, GameDeckRepository>();
            services.AddScoped<ITimeStatusRepository, TimeStatusRepository>();
            services.AddScoped<ISystemAnalyticsRepository, SystemAnalyticsRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IPenaltyInfoRepository, PenaltyInfoRepository>();
            services.AddScoped<IModerationRepository, ModerationRepository>();
            services.AddScoped<ICommandsAnalyticsRepository, CommandsAnalyticsRepository>();
            services.AddScoped<IUserAnalyticsRepository, UserAnalyticsRepository>();
            return services;
        }
    }
}
