﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;
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
        IEntityTypeConfiguration<WithoutMessageCountChannel>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
         
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
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GuildOptions)
                 .WithMany(g => g.Lands);
        }

        public void Configure(EntityTypeBuilder<WaifuCommandChannel> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Waifu)
                .WithMany(w => w.CommandChannels);
        }

        public void Configure(EntityTypeBuilder<WaifuFightChannel> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Waifu)
                .WithMany(w => w.FightChannels);
        }

        public void Configure(EntityTypeBuilder<CommandChannel> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.CommandChannels);
        }

        public void Configure(EntityTypeBuilder<WithoutMessageCountChannel> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.IgnoredChannels);
        }
    }
}