using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Explanation entity
/// </summary>
public class ExplanationConfiguration : AuditableEntityConfiguration<Explanation>
{
    public override void Configure(EntityTypeBuilder<Explanation> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.QuestionId)
            .IsRequired();

        builder.Property(e => e.AuthorId)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(e => e.Question)
            .WithMany(q => q.Explanations)
            .HasForeignKey(e => e.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Author)
            .WithMany(u => u.Explanations)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.QuestionId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.AuthorId)
            .HasFilter("[IsDeleted] = 0");
    }
}
