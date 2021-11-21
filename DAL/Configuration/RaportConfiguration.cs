using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.Configuration
{
    public class RaportConfiguration : IEntityTypeConfiguration<Raport>
    {
        public void Configure(EntityTypeBuilder<Raport> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.GuildOptions)
                .WithMany(g => g.Raports);
        }
    }
}
