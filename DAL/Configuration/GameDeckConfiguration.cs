using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.Configuration
{
    public class GameDeckConfiguration : IEntityTypeConfiguration<GameDeck>
    {
        public void Configure(EntityTypeBuilder<GameDeck> builder)
        {
            builder.HasKey(pr => pr.Id);
            builder.HasIndex(pr => pr.DeckPower);
            builder.HasIndex(pr => pr.WishlistIsPrivate);

            builder.HasOne(pr => pr.User)
                .WithOne(pr => pr.GameDeck);
        }
    }
}
