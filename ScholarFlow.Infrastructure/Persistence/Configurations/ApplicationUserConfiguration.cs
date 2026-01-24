using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // One-to-One relationships are configured in StudentProfile and TeacherProfile configurations

        builder.HasMany(u => u.CreatedPapers)
            .WithOne(p => p.Creator)
            .HasForeignKey(p => p.CreatedByTeacher)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Explanations)
            .WithOne(e => e.Author)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.ExamSessions)
            .WithOne(es => es.User)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
