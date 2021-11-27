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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasCharSet("utf8mb4", DelegationModes.ApplyToDatabases);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SanakanDbContext).Assembly);
        }
    }
}
