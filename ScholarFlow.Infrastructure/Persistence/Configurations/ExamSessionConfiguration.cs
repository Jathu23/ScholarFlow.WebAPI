using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

public class ExamSessionConfiguration : IEntityTypeConfiguration<ExamSession>
{
    public void Configure(EntityTypeBuilder<ExamSession> builder)
    {
        builder.HasKey(es => es.Id);

        builder.Property(es => es.StartTime)
            .IsRequired();

        builder.Property(es => es.FinalScore)
            .HasPrecision(5, 2);

        builder.Property(es => es.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(es => es.User)
            .WithMany(u => u.ExamSessions)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(es => es.Paper)
            .WithMany(p => p.ExamSessions)
            .HasForeignKey(es => es.PaperId)
            .OnDelete(DeleteBehavior.Restrict);

        // Performance indexes
        builder.HasIndex(es => es.UserId);
        
        builder.HasIndex(es => new { es.UserId, es.PaperId, es.StartTime });
        
        builder.HasIndex(es => es.Status);

        // Composite index for leaderboard queries
        builder.HasIndex(es => new { es.PaperId, es.FinalScore, es.Status });

        // Ignore computed property
        builder.Ignore(es => es.Duration);

        // Configure backing field
        builder.Metadata.FindNavigation(nameof(ExamSession.UserResponses))!
            .SetField("_userResponses");
    }
}
