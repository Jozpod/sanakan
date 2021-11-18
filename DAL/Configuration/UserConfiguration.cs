using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.Configuration
{
    public class UserConfiguration : 
        IEntityTypeConfiguration<User>,
        IEntityTypeConfiguration<UserStats>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(b => b.ShindenId);

            builder.HasCheckConstraint("CK_user_BackgroundProfileUri", "BackgroundProfileUri <> ''");
            builder.HasCheckConstraint("CK_user_StatsReplacementProfileUri", "StatsReplacementProfileUri <> ''");
        }

        public void Configure(EntityTypeBuilder<UserStats> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.User)
                .WithOne(u => u.Stats);
        }
    }
}
