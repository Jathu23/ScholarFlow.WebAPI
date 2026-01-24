using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Infrastructure.Persistence.Configurations;

/// <summary>
/// Base configuration for auditable entities
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt)
            .IsRequired(false);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .IsRequired();

        // Index for soft delete queries
        builder.HasIndex(e => e.IsDeleted)
            .HasFilter("[IsDeleted] = 0");

        // Index for audit queries
        builder.HasIndex(e => e.CreatedAt);
    }
}
