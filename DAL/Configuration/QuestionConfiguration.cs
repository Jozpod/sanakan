using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.Configuration
{
    public class QuestionConfiguration :
        IEntityTypeConfiguration<Question>,
        IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(e => e.Id);
        }

        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.Question)
                .WithMany(u => u.Answers);
        }
    }
}
