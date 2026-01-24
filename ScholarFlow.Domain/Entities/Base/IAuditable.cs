namespace ScholarFlow.Domain.Entities.Base;

/// <summary>
/// Interface for entities that support audit tracking
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}
