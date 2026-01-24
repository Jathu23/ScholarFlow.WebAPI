using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class SubTopicConfiguration : AuditableEntityConfiguration<SubTopic>
{
    public override void Configure(EntityTypeBuilder<SubTopic> builder)
    {
        base.Configure(builder);

        builder.Property(st => st.SubTopicName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(st => st.Topic)
            .WithMany(t => t.SubTopics)
            .HasForeignKey(st => st.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(st => new { st.TopicId, st.SubTopicName })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Configure backing field
        builder.Metadata.FindNavigation(nameof(SubTopic.Questions))!
            .SetField("_questions");
    }
}
