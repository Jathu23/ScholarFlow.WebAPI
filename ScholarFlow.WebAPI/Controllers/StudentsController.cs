using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Features.Students.Commands.CreateProfile;
using ScholarFlow.Application.Features.Students.Commands.UpdateProfile;
using ScholarFlow.Application.Features.Students.Queries.GetProfile;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Student Profile API Controller
/// </summary>
[ApiController]
[Route("api/students")]
[Authorize(Roles = "Student")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public StudentsController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    /// <summary>
    /// Create student profile
    /// </summary>
    [HttpPost("profile")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateStudentProfileCommand command, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Set UserId in command
        command.UserId = userId;

        // Delegate to handler
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get current student's profile
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Create query with UserId
        var query = new GetStudentProfileQuery { UserId = userId };

        // Delegate to handler
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Update student profile
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentProfileCommand command, CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Set UserId in command
        command.UserId = userId;

        // Delegate to handler
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get teachers connected/requested by current student
    /// </summary>
    [HttpGet("teachers")]
    public async Task<IActionResult> GetConnectedTeachers(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        var items = await _context.StudentTeacherConnections
            .Include(x => x.TeacherUser)
                .ThenInclude(u => u.TeacherProfile)
            .Include(x => x.Subject)
            .Where(x => x.StudentUserId == userId)
            .OrderByDescending(x => x.RequestedAt)
            .Select(x => new
            {
                teacherUserId = x.TeacherUserId,
                teacherCode = x.TeacherUser.TeacherProfile != null ? x.TeacherUser.TeacherProfile.TeacherCode : null,
                teacherName = x.TeacherUser.TeacherProfile != null
                    ? x.TeacherUser.TeacherProfile.FullName
                    : (x.TeacherUser.UserName ?? string.Empty),
                subjectId = x.SubjectId,
                subjectName = x.Subject.Name,
                connectedAt = x.RequestedAt,
                status = x.Status.ToString(),
                reviewedAt = x.ReviewedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    /// <summary>
    /// Request access to a teacher by teacher code for a selected subject
    /// </summary>
    [HttpPost("teachers/connect")]
    public async Task<IActionResult> ConnectTeacher([FromBody] ConnectTeacherRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        if (request.SubjectId == Guid.Empty || string.IsNullOrWhiteSpace(request.TeacherCode))
        {
            return BadRequest(new { error = "Teacher code and subject are required" });
        }

        var studentProfile = await _context.StudentProfiles
            .Include(x => x.SelectedSubjects)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (studentProfile == null)
        {
            return BadRequest(new { error = "Student profile not found. Please complete profile setup first." });
        }

        var selectedSubject = studentProfile.SelectedSubjects.Any(x => x.SubjectId == request.SubjectId);
        if (!selectedSubject)
        {
            return BadRequest(new { error = "Selected subject is not in your profile subjects." });
        }

        var normalizedCode = request.TeacherCode.Trim().ToUpperInvariant();
        var teacherProfile = await _context.TeacherProfiles
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TeacherCode != null
                     && x.TeacherCode.ToUpper() == normalizedCode
                     && x.Status == TeacherRegistrationStatus.Accepted,
                cancellationToken);

        if (teacherProfile == null)
        {
            return BadRequest(new { error = "Teacher ID is invalid or not approved yet." });
        }

        if (teacherProfile.SubjectId != request.SubjectId)
        {
            return BadRequest(new { error = "This teacher ID is not linked to the selected subject." });
        }

        var existing = await _context.StudentTeacherConnections
            .FirstOrDefaultAsync(
                x => x.StudentUserId == userId
                     && x.TeacherUserId == teacherProfile.UserId
                     && x.SubjectId == request.SubjectId,
                cancellationToken);

        if (existing == null)
        {
            existing = new Domain.Entities.StudentTeacherConnection
            {
                Id = Guid.NewGuid(),
                StudentUserId = userId,
                TeacherUserId = teacherProfile.UserId,
                SubjectId = request.SubjectId,
                Status = StudentTeacherConnectionStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _context.StudentTeacherConnections.Add(existing);
        }
        else if (existing.Status == StudentTeacherConnectionStatus.Rejected)
        {
            existing.Status = StudentTeacherConnectionStatus.Pending;
            existing.RequestedAt = DateTime.UtcNow;
            existing.ReviewedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            teacherUserId = teacherProfile.UserId,
            teacherCode = teacherProfile.TeacherCode,
            teacherName = teacherProfile.FullName,
            subjectId = request.SubjectId,
            subjectName = teacherProfile.SubjectId.HasValue
                ? await _context.Subjects.Where(s => s.Id == request.SubjectId).Select(s => s.Name).FirstOrDefaultAsync(cancellationToken) ?? string.Empty
                : string.Empty,
            connectedAt = existing.RequestedAt,
            status = existing.Status.ToString(),
            reviewedAt = existing.ReviewedAt
        });
    }

    /// <summary>
    /// Remove all teacher connections for a teacher from current student
    /// </summary>
    [HttpDelete("teachers/{teacherUserId:guid}")]
    public async Task<IActionResult> DisconnectTeacher(Guid teacherUserId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        var records = await _context.StudentTeacherConnections
            .Where(x => x.StudentUserId == userId && x.TeacherUserId == teacherUserId)
            .ToListAsync(cancellationToken);

        if (!records.Any())
        {
            return NotFound(new { error = "Connection not found" });
        }

        _context.StudentTeacherConnections.RemoveRange(records);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new { message = "Teacher disconnected successfully" });
    }
}

public class ConnectTeacherRequest
{
    public string TeacherCode { get; set; } = string.Empty;
    public Guid SubjectId { get; set; }
}
