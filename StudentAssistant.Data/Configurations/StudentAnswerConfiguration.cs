using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAssistant.Domain.Entities;

namespace StudentAssistant.Data.Configurations;

public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
{
    public void Configure(EntityTypeBuilder<StudentAnswer> builder)
    {
        builder.HasKey(sa => sa.Id);
        builder.HasOne(sa => sa.TestAttempt)
               .WithMany(t => t.StudentAnswers)
               .HasForeignKey(sa => sa.TestAttemptId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sa => sa.Question)
               .WithMany()
               .HasForeignKey(sa => sa.QuestionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sa => sa.SelectedOption)
               .WithMany()
               .HasForeignKey(sa => sa.SelectedOptionId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
