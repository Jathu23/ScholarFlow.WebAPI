using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class UserResponseConfiguration : IEntityTypeConfiguration<UserResponse>
{
    public void Configure(EntityTypeBuilder<UserResponse> builder)
    {
        builder.HasKey(ur => ur.Id);

        builder.Property(ur => ur.ResponseStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(ur => ur.TimeSpentSeconds)
            .IsRequired();

        builder.Property(ur => ur.IsCorrect)
            .IsRequired();

        builder.HasOne(ur => ur.Session)
            .WithMany(es => es.UserResponses)
            .HasForeignKey(ur => ur.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ur => ur.Question)
            .WithMany(q => q.UserResponses)
            .HasForeignKey(ur => ur.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ur => ur.SelectedOption)
            .WithMany(o => o.UserResponses)
            .HasForeignKey(ur => ur.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Performance indexes
        builder.HasIndex(ur => ur.SessionId);
        
        builder.HasIndex(ur => new { ur.SessionId, ur.QuestionId })
            .IsUnique();

        builder.HasIndex(ur => ur.ResponseStatus);

        // Index for analytics queries
        builder.HasIndex(ur => new { ur.QuestionId, ur.IsCorrect });

        // Index for review queries
        builder.HasIndex(ur => new { ur.SessionId, ur.ResponseStatus });
    }
}
