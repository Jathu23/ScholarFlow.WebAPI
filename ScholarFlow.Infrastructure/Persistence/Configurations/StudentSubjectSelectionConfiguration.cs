using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class StudentSubjectSelectionConfiguration : IEntityTypeConfiguration<StudentSubjectSelection>
{
    public void Configure(EntityTypeBuilder<StudentSubjectSelection> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.StudentProfile)
            .WithMany(x => x.SelectedSubjects)
            .HasForeignKey(x => x.StudentProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Subject)
            .WithMany()
            .HasForeignKey(x => x.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.StudentProfileId, x.SubjectId })
            .IsUnique();

        builder.HasIndex(x => x.SubjectId);
    }
}
