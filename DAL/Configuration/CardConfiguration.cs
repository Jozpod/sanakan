using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.Configuration
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(pr => pr.Id);
            builder.HasIndex(pr => pr.Active);

            builder.Property(pr => pr.CreatedOn)
                .HasColumnType("datetime(4)");

            builder.Property(pr => pr.ExpeditionDate)
               .HasColumnType("datetime(4)");

            builder
                .HasOne(pr => pr.GameDeck)
                .WithMany(pr => pr.Cards);

            builder.HasIndex(pr => pr.CharacterId);
            builder.HasIndex(pr => pr.GameDeckId);
            builder.HasIndex(pr => pr.Title);
        }
    }
}
