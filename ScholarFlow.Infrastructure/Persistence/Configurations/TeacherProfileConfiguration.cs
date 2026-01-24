using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class TeacherProfileConfiguration : IEntityTypeConfiguration<TeacherProfile>
{
    public void Configure(EntityTypeBuilder<TeacherProfile> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Qualification)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Bio)
            .HasMaxLength(2000);

        builder.HasOne(t => t.User)
            .WithOne(u => u.TeacherProfile)
            .HasForeignKey<TeacherProfile>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint on UserId
        builder.HasIndex(t => t.UserId)
            .IsUnique();
    }
}
