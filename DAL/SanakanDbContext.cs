using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;

namespace Sanakan.DAL
{
    public class SanakanDbContext : DbContext
    {
        private IOptionsMonitor<DatabaseConfiguration> _config;

        public SanakanDbContext(
            IOptionsMonitor<DatabaseConfiguration> config) : base()
        {
            _config = config;
        }

        #region Management
        public DbSet<PenaltyInfo> Penalties { get; set; }
        public DbSet<OwnedRole> OwnedRoles { get; set; }
        #endregion

        #region User
        public DbSet<User> Users { get; set; }
        public DbSet<UserStats> UsersStats { get; set; }
        public DbSet<TimeStatus> TimeStatuses { get; set; }
        public DbSet<SlotMachineConfig> SlotMachineConfigs { get; set; }
        public DbSet<GameDeck> GameDecks { get; set; }
        public DbSet<ExperienceContainer> ExpContainers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CardTag> CardTags { get; set; }
        public DbSet<BoosterPack> BoosterPacks { get; set; }
        public DbSet<CardPvPStats> CardPvPStats { get; set; }
        public DbSet<CardArenaStats> CardArenaStats { get; set; }
        public DbSet<BoosterPackCharacter> BoosterPackCharacters { get; set; }
        public DbSet<WishlistObject> Wishes { get; set; }
        public DbSet<Figure> Figures { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<RarityExcluded> RaritysExcludedFromPacks { get; set; }
        #endregion
        #region GuildConfig
        public DbSet<SelfRole> SelfRoles { get; set; }
        public DbSet<GuildOptions> Guilds { get; set; }
        public DbSet<LevelRole> LevelRoles { get; set; }
        public DbSet<CommandChannel> CommandChannels { get; set; }
        public DbSet<ModeratorRoles> ModeratorRoles { get; set; }
        public DbSet<WithoutExpChannel> WithoutExpChannels { get; set; }
        public DbSet<WithoutMessageCountChannel> IgnoredChannels { get; set; }
        public DbSet<WithoutSupervisionChannel> WithoutSupervisionChannels { get; set; }
        public DbSet<MyLand> MyLands { get; set; }
        public DbSet<WaifuConfiguration> Waifus { get; set; }
        public DbSet<Raport> Raports { get; set; }
        public DbSet<WaifuCommandChannel> WaifuCommandChannels { get; set; }
        public DbSet<WaifuFightChannel> WaifuFightChannels { get; set; }
        #endregion
        #region Analytics
        public DbSet<UserAnalytics> UsersData { get; set; }
        public DbSet<SystemAnalytics> SystemData { get; set; }
        public DbSet<TransferAnalytics> TransferData { get; set; }
        public DbSet<CommandsAnalytics> CommandsData { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = _config.CurrentValue;

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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (_config.CurrentValue.Provider == DatabaseProvider.MySql)
            {
                modelBuilder.HasCharSet("utf8mb4", DelegationModes.ApplyToDatabases);

                {
                    var builder = modelBuilder.Entity<Card>();
                    builder.HasCheckConstraint("CK_Card_Title", "Title <> '' OR Title IS NULL");
                    builder.HasCheckConstraint("CK_Card_ImageUrl", "ImageUrl <> '' OR ImageUrl IS NULL");
                    builder.HasCheckConstraint("CK_Card_CustomImageUrl", "CustomImageUrl <> '' OR CustomImageUrl IS NULL");
                    builder.HasCheckConstraint("CK_Card_CustomBorderUrl", "CustomBorderUrl <> '' OR CustomBorderUrl IS NULL");
                }

                {
                    var builder = modelBuilder.Entity<GameDeck>();
                    builder.HasCheckConstraint("CK_GameDeck_BackgroundImageUrl", "BackgroundImageUrl RLIKE'^http'");
                    builder.HasCheckConstraint("CK_GameDeck_ForegroundColor", "ForegroundColor RLIKE'^#[0-9]'");
                    builder.HasCheckConstraint("CK_GameDeck_ForegroundImageUrl", "ForegroundImageUrl RLIKE'^http'");
                }

                {
                    var builder = modelBuilder.Entity<User>();
                    builder.HasCheckConstraint("CK_User_BackgroundProfileUri", "BackgroundProfileUri <> ''");
                    builder.HasCheckConstraint("CK_User_StatsReplacementProfileUri", "StatsReplacementProfileUri <> ''");
                }
            }
                
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanakanDbContext).Assembly);
        }
    }
}
