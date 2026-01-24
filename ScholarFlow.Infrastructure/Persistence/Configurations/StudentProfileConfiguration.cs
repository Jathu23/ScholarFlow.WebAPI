using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Batch)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.District)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Medium)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(s => s.User)
            .WithOne(u => u.StudentProfile)
            .HasForeignKey<StudentProfile>(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Stream)
            .WithMany(st => st.Students)
            .HasForeignKey(s => s.StreamId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on UserId
        builder.HasIndex(s => s.UserId)
            .IsUnique();

        // Performance indexes
        builder.HasIndex(s => s.StreamId);
        
        builder.HasIndex(s => new { s.StreamId, s.Batch });
        
        builder.HasIndex(s => s.District);
    }
}
