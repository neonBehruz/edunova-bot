using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAssistant.Domain.Entities;

namespace StudentAssistant.Data.Configurations;

public class QuestionHistoryConfiguration : IEntityTypeConfiguration<QuestionHistory>
{
    public void Configure(EntityTypeBuilder<QuestionHistory> builder)
    {
        builder.HasKey(qh => qh.Id);
        builder.HasIndex(qh => new { qh.UserId, qh.QuestionId }).IsUnique();

        builder.HasOne(qh => qh.User)
               .WithMany()
               .HasForeignKey(qh => qh.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(qh => qh.Question)
               .WithMany()
               .HasForeignKey(qh => qh.QuestionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
