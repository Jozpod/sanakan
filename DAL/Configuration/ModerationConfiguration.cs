﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;

namespace Sanakan.DAL.Configuration
{
    public class ModerationConfiguration :
        IEntityTypeConfiguration<SelfRole>,
        IEntityTypeConfiguration<LevelRole>,
        IEntityTypeConfiguration<ModeratorRoles>,
        IEntityTypeConfiguration<PenaltyInfo>,
        IEntityTypeConfiguration<OwnedRole>
    {
        public void Configure(EntityTypeBuilder<SelfRole> builder)
        {
            builder.HasKey(pr => new { pr.RoleId, pr.GuildOptionsId });

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.SelfRoles);
        }

        public void Configure(EntityTypeBuilder<LevelRole> builder)
        {
            builder.HasKey(pr => new { pr.RoleId, pr.GuildOptionsId });

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.RolesPerLevel);
        }

        public void Configure(EntityTypeBuilder<ModeratorRoles> builder)
        {
            builder.HasKey(pr => new { pr.RoleId, pr.GuildOptionsId });

            builder
                .HasOne(e => e.GuildOptions)
                .WithMany(g => g.ModeratorRoles);
        }

        public void Configure(EntityTypeBuilder<PenaltyInfo> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.GuildId);
        }

        public void Configure(EntityTypeBuilder<OwnedRole> builder)
        {
            builder.HasKey(pr => new { pr.RoleId, pr.PenaltyInfoId });

            builder.HasOne(e => e.PenaltyInfo)
                .WithMany(p => p.Roles);
        }
    }
}
