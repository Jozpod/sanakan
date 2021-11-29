using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;

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
        public DbSet<UserLand> MyLands { get; set; }
        public DbSet<WaifuConfiguration> Waifus { get; set; }
        public DbSet<Report> Raports { get; set; }
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

                    builder.Property(pr => pr.ImageUrl)
                        .HasMaxLength(50)
                        .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                    builder.Property(pr => pr.CustomImageUrl)
                        .HasMaxLength(50)
                        .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                    builder.Property(pr => pr.CustomBorderUrl)
                        .HasMaxLength(50)
                        .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                    builder.HasCheckConstraint($"CK_{nameof(Card)}_{nameof(Card.Title)}", NotEmptyStringConstraint(nameof(Card.Title)));
                    builder.HasCheckConstraint($"CK_{nameof(Card)}_{nameof(Card.ImageUrl)}", NullableUrlConstraint(nameof(Card.ImageUrl)));
                    builder.HasCheckConstraint($"CK_{nameof(Card)}_{nameof(Card.CustomImageUrl)}", NullableUrlConstraint(nameof(Card.CustomImageUrl)));
                    builder.HasCheckConstraint($"CK_{nameof(Card)}_{nameof(Card.CustomBorderUrl)}", NullableUrlConstraint(nameof(Card.CustomBorderUrl)));
                }

                {
                    var builder = modelBuilder.Entity<GameDeck>();

                    builder.Property(pr => pr.BackgroundImageUrl)
                       .HasMaxLength(50)
                       .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                    builder.Property(pr => pr.ForegroundImageUrl)
                       .HasMaxLength(50)
                       .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                    builder.HasCheckConstraint($"CK_{nameof(GameDeck)}_{nameof(GameDeck.BackgroundImageUrl)}", NullableUrlConstraint(nameof(GameDeck.BackgroundImageUrl)));
                    builder.HasCheckConstraint($"CK_{nameof(GameDeck)}_{nameof(GameDeck.ForegroundColor)}", NullableColor(nameof(GameDeck.ForegroundColor)));
                    builder.HasCheckConstraint($"CK_{nameof(GameDeck)}_{nameof(GameDeck.ForegroundImageUrl)}", NullableUrlConstraint(nameof(GameDeck.ForegroundImageUrl)));
                }

                {
                    var builder = modelBuilder.Entity<User>();
                    builder.HasCheckConstraint($"CK_{nameof(User)}_{nameof(User.BackgroundProfileUri)}", NotEmptyStringConstraint(nameof(User.BackgroundProfileUri)));
                    builder.HasCheckConstraint($"CK_{nameof(User)}_{nameof(User.StatsReplacementProfileUri)}", NotEmptyStringConstraint(nameof(User.StatsReplacementProfileUri)));
                }
            }
                
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanakanDbContext).Assembly);
        }

        public string NullableColor(string property) => $"{property} REGEXP '^#([a-f0-9]{{3}}){{1,2}}$' OR {property} IS NULL";

        public string NotEmptyStringConstraint(string property) => $"TRIM({property}) <> '' OR {property} IS NULL";

        public string NullableUrlConstraint(string property)
        {
            return $"{property} REGEXP '^https?' OR {property} IS NULL";
        }
    }
}
