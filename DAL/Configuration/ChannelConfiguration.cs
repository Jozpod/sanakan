using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.Configuration
{
    public class ChannelConfiguration :
        IEntityTypeConfiguration<WithoutExpChannel>,
        IEntityTypeConfiguration<WithoutSupervisionChannel>,
        IEntityTypeConfiguration<UserLand>,
        IEntityTypeConfiguration<WaifuCommandChannel>,
        IEntityTypeConfiguration<WaifuFightChannel>,
        IEntityTypeConfiguration<CommandChannel>,
        IEntityTypeConfiguration<WithoutMessageCountChannel>
    {
        public void Configure(EntityTypeBuilder<WithoutExpChannel> builder)
        {
            builder.HasKey(pr => new { pr.ChannelId, pr.GuildOptionsId });

            builder
                .HasOne(e => e.GuildOptions)
                .WithMany(g => g.ChannelsWithoutExperience);
        }

        public void Configure(EntityTypeBuilder<WithoutSupervisionChannel> builder)
        {
            builder.HasKey(pr => new { pr.ChannelId, pr.GuildOptionsId });

            builder
                .HasOne(e => e.GuildOptions)
                .WithMany(g => g.ChannelsWithoutSupervision);
        }

        public void Configure(EntityTypeBuilder<UserLand> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GuildOptions)
                 .WithMany(g => g.Lands);
        }

        public void Configure(EntityTypeBuilder<WaifuCommandChannel> builder)
        {
            builder.HasKey(pr => new { pr.ChannelId, pr.WaifuId });

            builder.HasOne(e => e.Waifu)
                .WithMany(w => w.CommandChannels);
        }

        public void Configure(EntityTypeBuilder<WaifuFightChannel> builder)
        {
            builder.HasKey(pr => new { pr.ChannelId, pr.WaifuId });

            builder.HasOne(e => e.Waifu)
                .WithMany(w => w.FightChannels);
        }

        public void Configure(EntityTypeBuilder<CommandChannel> builder)
        {
            builder.HasKey(pr => new { pr.ChannelId, pr.GuildOptionsId });

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.CommandChannels);
        }

        public void Configure(EntityTypeBuilder<WithoutMessageCountChannel> builder)
        {
            builder.HasKey(pr => new { pr.ChannelId, pr.GuildOptionsId });

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.IgnoredChannels);
        }
    }
}
