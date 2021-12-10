using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.Configuration
{
    public class WishlistObjectConfiguration : IEntityTypeConfiguration<WishlistObject>
    {
        public void Configure(EntityTypeBuilder<WishlistObject> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GameDeck)
                .WithMany(d => d.Wishes);

            builder.HasAlternateKey(pr => new { pr.ObjectId, pr.GameDeckId });
            builder.HasIndex(pr => new { pr.Type, pr.ObjectId });
        }
    }
}
