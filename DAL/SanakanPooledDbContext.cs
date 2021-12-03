using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;

namespace Sanakan.DAL
{
    public class SanakanPooledDbContext : DbContext
    {
        private IOptionsMonitor<DatabaseConfiguration> _config;

        public SanakanPooledDbContext(DbContextOptions options) : base(options)
        {
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
        public DbSet<ExperienceContainer> ExpContainers { get; set; } = null!;
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
        public DbSet<UserLand> MyLands { get; set; } = null!;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasCharSet("utf8mb4", DelegationModes.ApplyToDatabases);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanakanDbContext).Assembly);
        }
    }
}
