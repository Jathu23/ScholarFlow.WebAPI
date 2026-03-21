using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OptionText)
            .IsRequired()
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(o => o.ContentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.ImageUrl)
            .HasColumnType("nvarchar(max)");

        builder.Property(o => o.Equation)
            .HasColumnType("nvarchar(max)");

        builder.Property(o => o.IsCorrect)
            .IsRequired();

        builder.HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.QuestionId);

        // Index for finding correct answers quickly
        builder.HasIndex(o => new { o.QuestionId, o.IsCorrect });

        // Configure backing field
        builder.Metadata.FindNavigation(nameof(Option.UserResponses))!
            .SetField("_userResponses");
    }
}
