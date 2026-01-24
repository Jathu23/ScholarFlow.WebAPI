using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class TopicConfiguration : AuditableEntityConfiguration<Topic>
{
    public override void Configure(EntityTypeBuilder<Topic> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.TopicName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(t => t.Subject)
            .WithMany(s => s.Topics)
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => new { t.SubjectId, t.TopicName })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Configure backing field
        builder.Metadata.FindNavigation(nameof(Topic.SubTopics))!
            .SetField("_subTopics");
    }
}
