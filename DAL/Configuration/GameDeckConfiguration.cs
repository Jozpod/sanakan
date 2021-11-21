using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.Configuration
{
    public class GameDeckConfiguration : IEntityTypeConfiguration<GameDeck>
    {
        public void Configure(EntityTypeBuilder<GameDeck> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.DeckPower);

            builder.HasOne(e => e.User)
                .WithOne(u => u.GameDeck);
        }
    }
}
