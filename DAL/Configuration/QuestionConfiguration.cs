using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Sanakan.DAL.Configuration
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
