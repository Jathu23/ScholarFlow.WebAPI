using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class PaperConfiguration : AuditableEntityConfiguration<Paper>
{
    public override void Configure(EntityTypeBuilder<Paper> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Year)
            .IsRequired();

        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(p => p.Subject)
            .WithMany(s => s.Papers)
            .HasForeignKey(p => p.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Creator)
            .WithMany(u => u.CreatedPapers)
            .HasForeignKey(p => p.CreatedByTeacher)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.SubjectId, p.Year, p.Type })
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(p => p.CreatedByTeacher);

        // Configure backing fields
        builder.Metadata.FindNavigation(nameof(Paper.Questions))!
            .SetField("_questions");
        builder.Metadata.FindNavigation(nameof(Paper.ExamSessions))!
            .SetField("_examSessions");
    }
}
