using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.Configuration
{
    public class TimeStatusConfiguration : IEntityTypeConfiguration<TimeStatus>
    {
        public void Configure(EntityTypeBuilder<TimeStatus> builder)
        {
            builder.HasKey(pr => pr.Id);
            builder.HasIndex(pr => pr.Type);

            builder.HasOne(pr => pr.User)
                .WithMany(pr => pr.TimeStatuses);
        }
    }
}
