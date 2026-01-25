using Microsoft.EntityFrameworkCore;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Entities.Base;

namespace ScholarFlow.Domain.Interfaces;

/// <summary>
/// Application DbContext interface for abstraction
/// </summary>
public interface IApplicationDbContext
{
    DbSet<AcademicStream> Streams { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<SubjectStream> SubjectStreams { get; }
    DbSet<Topic> Topics { get; }
    DbSet<SubTopic> SubTopics { get; }
    DbSet<Paper> Papers { get; }
    DbSet<Question> Questions { get; }
    DbSet<Option> Options { get; }
    DbSet<Explanation> Explanations { get; }
    DbSet<ExamSession> ExamSessions { get; }
    DbSet<UserResponse> UserResponses { get; }
    DbSet<StudentProfile> StudentProfiles { get; }
    DbSet<TeacherProfile> TeacherProfiles { get; }
    DbSet<ApplicationUser> Users { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
