using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using System;

namespace Sanakan.DAL.Configuration
{
    public class GameConfiguration : 
        IEntityTypeConfiguration<SlotMachineConfig>,
        IEntityTypeConfiguration<TimeStatus>,
        IEntityTypeConfiguration<GameDeck>,
        IEntityTypeConfiguration<ExpContainer>,
        IEntityTypeConfiguration<Figure>,
        IEntityTypeConfiguration<Card>,
        IEntityTypeConfiguration<Item>,
        IEntityTypeConfiguration<WishlistObject>,
        IEntityTypeConfiguration<BoosterPack>,
        IEntityTypeConfiguration<CardPvPStats>,
        IEntityTypeConfiguration<CardTag>,
        IEntityTypeConfiguration<CardArenaStats>,
        IEntityTypeConfiguration<BoosterPackCharacter>,
        IEntityTypeConfiguration<RarityExcluded>,
        IEntityTypeConfiguration<GuildOptions>,
        IEntityTypeConfiguration<WaifuConfiguration>,
        IEntityTypeConfiguration<Raport>
    {
        public void Configure(EntityTypeBuilder<Raport> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.Raports);
        }

        public void Configure(EntityTypeBuilder<SlotMachineConfig> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.User)
                .WithOne(u => u.SMConfig);

        }

        public void Configure(EntityTypeBuilder<TimeStatus> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.User)
                .WithMany(u => u.TimeStatuses);
        }

        public void Configure(EntityTypeBuilder<GameDeck> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.DeckPower);

            builder.HasOne(e => e.User)
                .WithOne(u => u.GameDeck);
        }

        public void Configure(EntityTypeBuilder<ExpContainer> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithOne(u => u.ExpContainer);
        }

        public void Configure(EntityTypeBuilder<Figure> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithMany(u => u.Figures);
        }

        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.Active);

            builder.HasOne(e => e.GameDeck)
                .WithMany(d => d.Cards);

            builder.HasIndex(b => b.CharacterId);
            builder.HasIndex(b => b.GameDeckId);
            builder.HasIndex(b => b.Title);
        }

        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithMany(d => d.Items);
        }

        public void Configure(EntityTypeBuilder<WishlistObject> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithMany(d => d.Wishes);
        }

        public void Configure(EntityTypeBuilder<BoosterPack> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithMany(d => d.BoosterPacks);
        }

        public void Configure(EntityTypeBuilder<CardPvPStats> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithMany(d => d.PvPStats);
        }

        public void Configure(EntityTypeBuilder<CardTag> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Card)
                .WithMany(d => d.TagList);
        }

        public void Configure(EntityTypeBuilder<CardArenaStats> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Card)
                .WithOne(c => c.ArenaStats);
        }

        public void Configure(EntityTypeBuilder<BoosterPackCharacter> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.BoosterPack)
                .WithMany(p => p.Characters);
        }

        public void Configure(EntityTypeBuilder<RarityExcluded> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.BoosterPack)
                .WithMany(p => p.RarityExcludedFromPack);
        }

        public void Configure(EntityTypeBuilder<GuildOptions> builder)
        {
            builder.HasKey(e => e.Id);
        }

        public void Configure(EntityTypeBuilder<WaifuConfiguration> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasMany(e => e.CommandChannels)
                .WithOne(w => w.Waifu);
            
            builder
                .HasMany(e => e.FightChannels)
                .WithOne(w => w.Waifu);
            
            builder
                .HasOne(e => e.GuildOptions)
                .WithOne(g => g.WaifuConfig);
        }
    }
}
