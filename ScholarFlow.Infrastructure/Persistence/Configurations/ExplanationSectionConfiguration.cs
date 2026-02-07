using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ExplanationSection entity
/// </summary>
public class ExplanationSectionConfiguration : IEntityTypeConfiguration<ExplanationSection>
{
    public void Configure(EntityTypeBuilder<ExplanationSection> builder)
    {
        builder.ToTable("ExplanationSections");

        builder.HasKey(es => es.Id);

        builder.Property(es => es.ExplanationId)
            .IsRequired();

        builder.Property(es => es.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(es => es.Content)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(es => es.Caption)
            .HasMaxLength(500);

        builder.Property(es => es.OrderIndex)
            .IsRequired();

        // Relationship with Explanation
        builder.HasOne(es => es.Explanation)
            .WithMany(e => e.Sections)
            .HasForeignKey(es => es.ExplanationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for ordering
        builder.HasIndex(es => new { es.ExplanationId, es.OrderIndex })
            .HasDatabaseName("IX_ExplanationSections_ExplanationId_OrderIndex");
    }
}
