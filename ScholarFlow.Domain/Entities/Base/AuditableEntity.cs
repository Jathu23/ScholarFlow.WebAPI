namespace ScholarFlow.Domain.Entities.Base;

/// <summary>
/// Base entity with audit tracking capabilities
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User ID who created the entity
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when the entity was last modified
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User ID who last modified the entity
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Soft delete flag - if true, entity is considered deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date and time when the entity was soft deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User ID who deleted the entity
    /// </summary>
    public Guid? DeletedBy { get; set; }

    /// <summary>
    /// Concurrency token for optimistic concurrency control
    /// </summary>
    public byte[] RowVersion { get; set; } = null!;
}
