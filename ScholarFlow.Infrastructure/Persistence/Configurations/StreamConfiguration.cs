using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class AcademicStreamConfiguration : AuditableEntityConfiguration<AcademicStream>
{
    public override void Configure(EntityTypeBuilder<AcademicStream> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Configure backing fields for navigation properties
        builder.Metadata.FindNavigation(nameof(AcademicStream.Students))!
            .SetField("_students");
        builder.Metadata.FindNavigation(nameof(AcademicStream.SubjectStreams))!
            .SetField("_subjectStreams");
    }
}
