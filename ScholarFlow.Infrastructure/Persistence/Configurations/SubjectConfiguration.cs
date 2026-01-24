using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class SubjectConfiguration : AuditableEntityConfiguration<Subject>
{
    public override void Configure(EntityTypeBuilder<Subject> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Configure backing fields
        builder.Metadata.FindNavigation(nameof(Subject.SubjectStreams))!
            .SetField("_subjectStreams");
        builder.Metadata.FindNavigation(nameof(Subject.Topics))!
            .SetField("_topics");
        builder.Metadata.FindNavigation(nameof(Subject.Papers))!
            .SetField("_papers");
    }
}
