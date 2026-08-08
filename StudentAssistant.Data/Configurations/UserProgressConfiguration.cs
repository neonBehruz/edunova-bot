using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAssistant.Domain.Entities;

namespace StudentAssistant.Data.Configurations;

public class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.HasKey(up => up.Id);
        builder.HasIndex(up => new { up.UserId, up.SubjectId, up.Level }).IsUnique();

        builder.HasOne(up => up.User)
               .WithMany(u => u.Progresses)
               .HasForeignKey(up => up.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Subject)
               .WithMany()
               .HasForeignKey(up => up.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
