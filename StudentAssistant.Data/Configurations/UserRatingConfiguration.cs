using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAssistant.Domain.Entities;

namespace StudentAssistant.Data.Configurations;

public class UserRatingConfiguration : IEntityTypeConfiguration<UserRating>
{
    public void Configure(EntityTypeBuilder<UserRating> builder)
    {
        builder.HasKey(ur => ur.Id);
        builder.HasOne(ur => ur.User)
               .WithOne(u => u.Rating)
               .HasForeignKey<UserRating>(ur => ur.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
