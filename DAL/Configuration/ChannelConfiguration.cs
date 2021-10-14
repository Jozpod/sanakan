using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models.Configuration;
using System;

namespace Sanakan.DAL.Configuration
{
    public class ChannelConfiguration : 
        IEntityTypeConfiguration<WithoutExpChannel>,
        IEntityTypeConfiguration<WithoutSupervisionChannel>,
        IEntityTypeConfiguration<MyLand>,
        IEntityTypeConfiguration<WaifuCommandChannel>,
        IEntityTypeConfiguration<WaifuFightChannel>,
        IEntityTypeConfiguration<CommandChannel>,
        IEntityTypeConfiguration<WithoutMsgCntChannel>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            modelBuilder.Entity<MyLand>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.GuildOptions)
                     .WithMany(g => g.Lands);
            });

            modelBuilder.Entity<WaifuCommandChannel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Waifu)
                    .WithMany(w => w.CommandChannels);
            });

            modelBuilder.Entity<WaifuFightChannel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Waifu)
                    .WithMany(w => w.FightChannels);
            });

            modelBuilder.Entity<CommandChannel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.GuildOptions)
                    .WithMany(g => g.CommandChannels);
            });

            modelBuilder.Entity<WithoutMsgCntChannel>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.GuildOptions)
                    .WithMany(g => g.IgnoredChannels);
            });
        }

        public void Configure(EntityTypeBuilder<WithoutExpChannel> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.GuildOptions)
                .WithMany(g => g.ChannelsWithoutExp);
        }

        public void Configure(EntityTypeBuilder<WithoutSupervisionChannel> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.GuildOptions)
                .WithMany(g => g.ChannelsWithoutSupervision);
        }

        public void Configure(EntityTypeBuilder<MyLand> builder)
        {
            throw new NotImplementedException();
        }

        public void Configure(EntityTypeBuilder<WaifuCommandChannel> builder)
        {
            throw new NotImplementedException();
        }

        public void Configure(EntityTypeBuilder<WaifuFightChannel> builder)
        {
            throw new NotImplementedException();
        }

        public void Configure(EntityTypeBuilder<CommandChannel> builder)
        {
            throw new NotImplementedException();
        }

        public void Configure(EntityTypeBuilder<WithoutMsgCntChannel> builder)
        {
            throw new NotImplementedException();
        }
    }
}
