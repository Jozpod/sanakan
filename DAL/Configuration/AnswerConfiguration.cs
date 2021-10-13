using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.Database.Models;
using System;

namespace Sanakan.DAL.Configuration
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.Question)
                .WithMany(u => u.Answers);
        }
    }
}
