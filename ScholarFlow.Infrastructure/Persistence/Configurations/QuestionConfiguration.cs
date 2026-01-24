using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class QuestionConfiguration : AuditableEntityConfiguration<Question>
{
    public override void Configure(EntityTypeBuilder<Question> builder)
    {
        base.Configure(builder);

        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(q => q.QuestionImageUrl)
            .HasMaxLength(500);

        builder.Property(q => q.Difficulty)
            .IsRequired();

        builder.HasOne(q => q.Paper)
            .WithMany(p => p.Questions)
            .HasForeignKey(q => q.PaperId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.SubTopic)
            .WithMany(st => st.Questions)
            .HasForeignKey(q => q.SubTopicId)
            .OnDelete(DeleteBehavior.Restrict);

        // Performance indexes
        builder.HasIndex(q => q.PaperId)
            .HasFilter("[IsDeleted] = 0");
        
        builder.HasIndex(q => q.SubTopicId)
            .HasFilter("[IsDeleted] = 0");
        
        builder.HasIndex(q => q.Difficulty)
            .HasFilter("[IsDeleted] = 0");

        // Composite index for common queries
        builder.HasIndex(q => new { q.SubTopicId, q.Difficulty })
            .HasFilter("[IsDeleted] = 0");

        // Configure backing fields
        builder.Metadata.FindNavigation(nameof(Question.Options))!
            .SetField("_options");
        builder.Metadata.FindNavigation(nameof(Question.Explanations))!
            .SetField("_explanations");
        builder.Metadata.FindNavigation(nameof(Question.UserResponses))!
            .SetField("_userResponses");
    }
}
