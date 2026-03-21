using ScholarFlow.Domain.Entities.Base;
using ScholarFlow.Domain.Enums;

namespace ScholarFlow.Domain.Entities;

public class StudentTeacherConnection : BaseEntity
{
    public Guid StudentUserId { get; set; }
    public Guid TeacherUserId { get; set; }
    public Guid SubjectId { get; set; }
    public StudentTeacherConnectionStatus Status { get; set; } = StudentTeacherConnectionStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public ApplicationUser StudentUser { get; set; } = null!;
    public ApplicationUser TeacherUser { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
}
