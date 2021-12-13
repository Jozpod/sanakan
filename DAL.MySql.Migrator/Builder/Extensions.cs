using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.MySql.Migrator;
using Sanakan.DAL.MySql.Migrator.Configuration;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DAL.MySql.Builder
{
    [ExcludeFromCodeCoverage]
    public static class Extensions
    {
        public static IServiceCollection AddDatabaseMigrator(this IServiceCollection services)
        {
            services.AddScoped<DatabaseMigrator>();
            services.AddScoped<TableEnumerator<GuildOptions>, GuildOptionsEnumerator>();
            services.AddScoped<TableEnumerator<WaifuConfiguration>, WaifuConfigurationEnumerator>();
            services.AddScoped<TableEnumerator<Answer>, AnswersEnumerator>();
            services.AddScoped<TableEnumerator<Question>, QuestionsEnumerator>();
            services.AddScoped<TableEnumerator<Report>, ReportsEnumerator>();
            services.AddScoped<TableEnumerator<TimeStatus>, TimeStatusEnumerator>();
            services.AddScoped<TableEnumerator<Item>, ItemEnumerator>();
            services.AddScoped<TableEnumerator<GameDeck>, GameDecksEnumerators>();
            services.AddScoped<TableEnumerator<Figure>, FiguresEnumerator>();
            services.AddScoped<TableEnumerator<RarityExcluded>, RarityExcludedEnumerator>();
            services.AddScoped<TableEnumerator<User>, UserEnumerator>();
            services.AddScoped<TableEnumerator<GameDeck>, GameDecksEnumerators>();
            services.AddScoped<TableEnumerator<CardArenaStats>, CardArenaStatsEnumerator>();
            services.AddScoped<TableEnumerator<CardPvPStats>, CardPvPStatsEnumerator>();
            services.AddScoped<TableEnumerator<CardTag>, CardTagsEnumerator>();
            services.AddScoped<TableEnumerator<Card>, CardEnumerator>();
            services.AddScoped<TableEnumerator<UserLand>, UserLandEnumerator>();
            services.AddScoped<TableEnumerator<BoosterPack>, BoosterPackEnumerator>();
            services.AddScoped<TableEnumerator<BoosterPackCharacter>, BoosterPackCharacterEnumerator>();
            services.AddScoped<TableEnumerator<CommandsAnalytics>, CommandsAnalyticsEnumerator>();
            services.AddScoped<TableEnumerator<TransferAnalytics>, TransferAnalyticsEnumerator>();
            services.AddScoped<TableEnumerator<SystemAnalytics>, SystemAnalyticsEnumerator>();
            services.AddScoped<TableEnumerator<UserAnalytics>, UserAnalyticsEnumerator>();
            services.AddScoped<TableEnumerator<SlotMachineConfig>, SlotMachineConfigEnumerator>();
            services.AddScoped<TableEnumerator<ExperienceContainer>, ExperienceContainerEnumerator>();
            services.AddScoped<TableEnumerator<CommandChannel>, CommandChannelEnumerator>();
            services.AddScoped<TableEnumerator<ModeratorRoles>, ModeratorRolesEnumerator>();
            services.AddScoped<TableEnumerator<LevelRole>, LevelRolesEnumerator>();
            services.AddScoped<TableEnumerator<SelfRole>, SelfRolesEnumerator>();
            services.AddScoped<TableEnumerator<OwnedRole>, OwnedRolesEnumerator>();
            services.AddScoped<TableEnumerator<PenaltyInfo>, PenaltiesEnumerator>();
            services.AddScoped<TableEnumerator<UserStats>, UserStatsEnumerator>();
            services.AddScoped<TableEnumerator<WaifuFightChannel>, WaifuFightChannelEnumerator>();
            services.AddScoped<TableEnumerator<WishlistObject>, WishlistObjectEnumerator>();
            services.AddScoped<TableEnumerator<WithoutExpChannel>, WithoutExpChannelEnumerator>();
            services.AddScoped<TableEnumerator<WithoutSupervisionChannel>, WithoutSupervisionChannelEnumerator>();
            services.AddScoped<TableEnumerator<WithoutMessageCountChannel>, WithoutMessageCountChannelEnumerator>();
            services.AddScoped<TableEnumerator<WaifuCommandChannel>, WaifuCommandChannelEnumerator>();
            services.AddScoped<IDbConnection>((serviceProvider) =>
            {
                var configuration = serviceProvider.GetRequiredService<IOptions<SourceDatabaseConfiguration>>().Value;
                var connection = new MySqlConnection(configuration.ConnectionString);
                return new DbConnection(connection);
            });
            return services;
        }
    }
}
