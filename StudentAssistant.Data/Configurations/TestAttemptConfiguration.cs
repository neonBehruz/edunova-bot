using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAssistant.Domain.Entities;

namespace StudentAssistant.Data.Configurations;

public class TestAttemptConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.User)
               .WithMany(u => u.TestAttempts)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Subject)
               .WithMany()
               .HasForeignKey(t => t.SubjectId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
