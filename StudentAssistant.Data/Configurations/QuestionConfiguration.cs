using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAssistant.Domain.Entities;

namespace StudentAssistant.Data.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).IsRequired().HasMaxLength(1000);
        builder.HasOne(q => q.Subject)
               .WithMany(s => s.Questions)
               .HasForeignKey(q => q.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
