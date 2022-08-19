using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.DAL
{
    [ExcludeFromCodeCoverage]
    public class SanakanDbContext : DbContext
    {
        private IOptionsMonitor<DatabaseConfiguration> _config;

        public SanakanDbContext(
            IOptionsMonitor<DatabaseConfiguration> config)
            : base()
        {
            _config = config;
        }

        #region Management
        public DbSet<PenaltyInfo> Penalties { get; set; } = null!;

        public DbSet<OwnedRole> OwnedRoles { get; set; } = null!;
        #endregion

        #region User
        public DbSet<User> Users { get; set; } = null!;

        public DbSet<UserStats> UsersStats { get; set; } = null!;

        public DbSet<TimeStatus> TimeStatuses { get; set; } = null!;

        public DbSet<SlotMachineConfig> SlotMachineConfigs { get; set; } = null!;

        public DbSet<GameDeck> GameDecks { get; set; } = null!;

        public DbSet<ExperienceContainer> ExperienceContainers { get; set; } = null!;

        public DbSet<Card> Cards { get; set; } = null!;

        public DbSet<Item> Items { get; set; } = null!;

        public DbSet<CardTag> CardTags { get; set; } = null!;

        public DbSet<BoosterPack> BoosterPacks { get; set; } = null!;

        public DbSet<CardPvPStats> CardPvPStats { get; set; } = null!;

        public DbSet<CardArenaStats> CardArenaStats { get; set; } = null!;

        public DbSet<BoosterPackCharacter> BoosterPackCharacters { get; set; } = null!;

        public DbSet<WishlistObject> Wishes { get; set; } = null!;

        public DbSet<Figure> Figures { get; set; } = null!;

        public DbSet<Question> Questions { get; set; } = null!;

        public DbSet<Answer> Answers { get; set; } = null!;

        public DbSet<RarityExcluded> RaritysExcludedFromPacks { get; set; } = null!;
        #endregion

        #region GuildConfig
        public DbSet<SelfRole> SelfRoles { get; set; } = null!;

        public DbSet<GuildOptions> Guilds { get; set; } = null!;

        public DbSet<LevelRole> LevelRoles { get; set; } = null!;

        public DbSet<CommandChannel> CommandChannels { get; set; } = null!;

        public DbSet<ModeratorRoles> ModeratorRoles { get; set; } = null!;

        public DbSet<WithoutExpChannel> WithoutExpChannels { get; set; } = null!;

        public DbSet<WithoutMessageCountChannel> IgnoredChannels { get; set; } = null!;

        public DbSet<WithoutSupervisionChannel> WithoutSupervisionChannels { get; set; } = null!;

        public DbSet<UserLand> UserLands { get; set; } = null!;

        public DbSet<WaifuConfiguration> Waifus { get; set; } = null!;

        public DbSet<Report> Raports { get; set; } = null!;

        public DbSet<WaifuCommandChannel> WaifuCommandChannels { get; set; } = null!;

        public DbSet<WaifuFightChannel> WaifuFightChannels { get; set; } = null!;
        #endregion

        #region Analytics
        public DbSet<UserAnalytics> UsersData { get; set; } = null!;

        public DbSet<SystemAnalytics> SystemData { get; set; } = null!;

        public DbSet<TransferAnalytics> TransferData { get; set; } = null!;

        public DbSet<CommandsAnalytics> CommandsData { get; set; } = null!;
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = _config.CurrentValue;

            if (config.Provider == DatabaseProvider.InMemory)
            {
                optionsBuilder.UseInMemoryDatabase("SanakanDb");
            }

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
                OnMySqlModelCreating(modelBuilder);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanakanDbContext).Assembly);
        }

        #region MySqlModelCreating
        private void OnMySqlModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8mb4", DelegationModes.ApplyToDatabases);
            {
                var builder = modelBuilder.Entity<Card>();

                builder.Property(pr => pr.ImageUrl)
                    .HasMaxLength(90)
                    .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                builder.Property(pr => pr.CustomImageUrl)
                    .HasMaxLength(90)
                    .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                builder.Property(pr => pr.CustomBorderUrl)
                    .HasMaxLength(90)
                    .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                builder.HasCheckConstraint($"{nameof(Card.Title)}", NotEmptyStringConstraint(nameof(Card.Title)));
                builder.HasCheckConstraint($"{nameof(Card.ImageUrl)}", NullableUrlConstraint(nameof(Card.ImageUrl)));
                builder.HasCheckConstraint($"{nameof(Card.CustomImageUrl)}", NullableUrlConstraint(nameof(Card.CustomImageUrl)));
                builder.HasCheckConstraint($"{nameof(Card.CustomBorderUrl)}", NullableUrlConstraint(nameof(Card.CustomBorderUrl)));
            }

            {
                var builder = modelBuilder.Entity<GameDeck>();

                builder.Property(pr => pr.BackgroundImageUrl)
                   .HasMaxLength(90)
                   .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                builder.Property(pr => pr.ForegroundImageUrl)
                   .HasMaxLength(90)
                   .HasConversion(pr => pr == null ? null : pr.ToString(), pr => pr == null ? null : new Uri(pr));

                builder.HasCheckConstraint($"{nameof(GameDeck.BackgroundImageUrl)}", NullableUrlConstraint(nameof(GameDeck.BackgroundImageUrl)));
                builder.HasCheckConstraint($"{nameof(GameDeck.ForegroundColor)}", NullableColor(nameof(GameDeck.ForegroundColor)));
                builder.HasCheckConstraint($"{nameof(GameDeck.ForegroundImageUrl)}", NullableUrlConstraint(nameof(GameDeck.ForegroundImageUrl)));
            }

            {
                var builder = modelBuilder.Entity<User>();
                builder.HasCheckConstraint($"{nameof(User.BackgroundProfileUri)}", NotEmptyStringConstraint(nameof(User.BackgroundProfileUri)));
                builder.HasCheckConstraint($"{nameof(User.StatsReplacementProfileUri)}", NotEmptyStringConstraint(nameof(User.StatsReplacementProfileUri)));
            }
        }

        private string NullableColor(string property) => $"{property} REGEXP '^#([a-f0-9]{{3}}){{1,2}}$' OR {property} IS NULL";

        private string NotEmptyStringConstraint(string property) => $"TRIM({property}) <> '' OR {property} IS NULL";

        private string NullableUrlConstraint(string property) => $"{property} REGEXP '^https?' OR {property} IS NULL";
        #endregion
    }
}
