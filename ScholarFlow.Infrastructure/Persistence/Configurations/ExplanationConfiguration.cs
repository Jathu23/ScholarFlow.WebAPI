using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class ExplanationConfiguration : AuditableEntityConfiguration<Explanation>
{
    public override void Configure(EntityTypeBuilder<Explanation> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasColumnType("NVARCHAR(MAX)");

        builder.HasOne(e => e.Question)
            .WithMany(q => q.Explanations)
            .HasForeignKey(e => e.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Author)
            .WithMany(u => u.Explanations)
            .HasForeignKey(e => e.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.QuestionId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => e.AuthorId)
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(e => new { e.QuestionId, e.Type })
            .HasFilter("[IsDeleted] = 0");
    }
}
