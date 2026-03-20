using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class StudentTeacherConnectionConfiguration : IEntityTypeConfiguration<StudentTeacherConnection>
{
    public void Configure(EntityTypeBuilder<StudentTeacherConnection> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(x => x.RequestedAt)
            .IsRequired();

        builder.HasOne(x => x.StudentUser)
            .WithMany()
            .HasForeignKey(x => x.StudentUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TeacherUser)
            .WithMany()
            .HasForeignKey(x => x.TeacherUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Subject)
            .WithMany()
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.StudentUserId, x.TeacherUserId, x.SubjectId })
            .IsUnique();
        builder.HasIndex(x => new { x.TeacherUserId, x.Status });
        builder.HasIndex(x => new { x.StudentUserId, x.Status });
    }
}
