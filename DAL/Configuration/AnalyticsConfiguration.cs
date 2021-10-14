using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models.Analytics;
using System;

namespace Sanakan.DAL.Configuration
{
    public class AnalyticsConfiguration : 
        IEntityTypeConfiguration<UserAnalytics>,
        IEntityTypeConfiguration<SystemAnalytics>,
        IEntityTypeConfiguration<TransferAnalytics>,
        IEntityTypeConfiguration<CommandsAnalytics>
    {
        public void Configure(EntityTypeBuilder<UserAnalytics> builder)
        {
            builder.HasKey(e => e.Id);
        }

        public void Configure(EntityTypeBuilder<SystemAnalytics> builder)
        {
            builder.HasKey(e => e.Id);
        }

        public void Configure(EntityTypeBuilder<TransferAnalytics> builder)
        {
            builder.HasKey(e => e.Id);
        }

        public void Configure(EntityTypeBuilder<CommandsAnalytics> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
