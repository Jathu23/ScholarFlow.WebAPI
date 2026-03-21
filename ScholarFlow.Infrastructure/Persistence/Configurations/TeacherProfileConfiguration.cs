using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Enums;

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

        builder.Property(t => t.PhoneNumber)
            .HasMaxLength(50);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(TeacherRegistrationStatus.Pending);

        builder.Property(t => t.TeacherCode)
            .HasMaxLength(50);

        builder.Property(t => t.RejectionReason)
            .HasMaxLength(1000);

        builder.HasOne(t => t.User)
            .WithOne(u => u.TeacherProfile)
            .HasForeignKey<TeacherProfile>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Subject)
            .WithMany()
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on UserId
        builder.HasIndex(t => t.UserId)
            .IsUnique();

        builder.HasIndex(t => t.Status);

        builder.HasIndex(t => t.TeacherCode)
            .IsUnique()
            .HasFilter("[TeacherCode] IS NOT NULL");
    }
}
