using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScholarFlow.Application.Features.Teachers.Commands.CreateProfile;
using ScholarFlow.Application.Features.Teachers.Commands.UpdateProfile;
using ScholarFlow.Application.Features.Teachers.Queries.GetProfile;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Teacher Profile API Controller
/// </summary>
[ApiController]
[Route("api/teachers")]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public TeachersController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public class ReviewTeacherStatusRequest
    {
        public int Status { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class ReviewStudentConnectionStatusRequest
    {
        public int Status { get; set; }
    }

    /// <summary>
    /// Create teacher profile
    /// </summary>
    [HttpPost("profile")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateTeacherProfileCommand command, CancellationToken cancellationToken)
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
    /// Get current teacher's profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        // Extract UserId from JWT token
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        // Create query with UserId
        var query = new GetTeacherProfileQuery { UserId = userId };

        // Delegate to handler
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Update teacher profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateTeacherProfileCommand command, CancellationToken cancellationToken)
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
    /// Get teacher registration requests for admin review
    /// </summary>
    [HttpGet("admin/requests")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTeacherRequests([FromQuery] string? status, CancellationToken cancellationToken)
    {
        TeacherRegistrationStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<TeacherRegistrationStatus>(status, true, out var parsed))
        {
            statusFilter = parsed;
        }

        var query = _context.TeacherProfiles
            .Include(t => t.User)
            .Include(t => t.Subject)
            .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(t => t.Status == statusFilter.Value);
        }

        var requests = await query
            .OrderBy(t => t.FullName)
            .Select(t => new
            {
                userId = t.UserId,
                profileId = t.Id,
                email = t.User.Email,
                fullName = t.FullName,
                qualification = t.Qualification,
                subjectId = t.SubjectId,
                subjectName = t.Subject != null ? t.Subject.Name : string.Empty,
                phoneNumber = t.PhoneNumber,
                status = t.Status.ToString(),
                teacherCode = t.TeacherCode,
                paperCount = _context.Papers.Count(p => p.CreatedByTeacher == t.UserId),
                rejectionReason = t.RejectionReason,
                reviewedAt = t.ReviewedAt,
            })
            .ToListAsync(cancellationToken);

        return Ok(requests);
    }

    /// <summary>
    /// Review teacher registration request (accept/reject)
    /// </summary>
    [HttpPut("admin/{teacherUserId:guid}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReviewTeacherRequest(Guid teacherUserId, [FromBody] ReviewTeacherStatusRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(TeacherRegistrationStatus), request.Status))
        {
            return BadRequest(new { error = "Invalid status value" });
        }

        var profile = await _context.TeacherProfiles
            .FirstOrDefaultAsync(t => t.UserId == teacherUserId, cancellationToken);

        if (profile == null)
        {
            return NotFound(new { error = "Teacher profile not found" });
        }

        var nextStatus = (TeacherRegistrationStatus)request.Status;
        profile.Status = nextStatus;
        profile.ReviewedAt = DateTime.UtcNow;

        if (nextStatus == TeacherRegistrationStatus.Accepted)
        {
            profile.RejectionReason = null;
            if (string.IsNullOrWhiteSpace(profile.TeacherCode))
            {
                profile.TeacherCode = $"TCH-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
            }
        }
        else if (nextStatus == TeacherRegistrationStatus.Rejected)
        {
            profile.RejectionReason = request.RejectionReason?.Trim();
            profile.TeacherCode = null;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            userId = profile.UserId,
            profileId = profile.Id,
            status = profile.Status.ToString(),
            teacherCode = profile.TeacherCode,
            rejectionReason = profile.RejectionReason,
            reviewedAt = profile.ReviewedAt,
        });
    }

    /// <summary>
    /// Get student access requests sent to current teacher
    /// </summary>
    [HttpGet("student-requests")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetStudentRequests([FromQuery] string? status, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var teacherUserId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        StudentTeacherConnectionStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StudentTeacherConnectionStatus>(status, true, out var parsed))
        {
            statusFilter = parsed;
        }

        var query = _context.StudentTeacherConnections
            .Include(x => x.StudentUser)
                .ThenInclude(u => u.StudentProfile)
            .Include(x => x.Subject)
            .Where(x => x.TeacherUserId == teacherUserId)
            .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(x => x.Status == statusFilter.Value);
        }

        var requests = await query
            .OrderByDescending(x => x.RequestedAt)
            .Select(x => new
            {
                requestId = x.Id,
                studentUserId = x.StudentUserId,
                studentName = x.StudentUser.StudentProfile != null
                    ? x.StudentUser.StudentProfile.FullName
                    : (x.StudentUser.UserName ?? string.Empty),
                studentEmail = x.StudentUser.Email,
                subjectId = x.SubjectId,
                subjectName = x.Subject.Name,
                status = x.Status.ToString(),
                requestedAt = x.RequestedAt,
                reviewedAt = x.ReviewedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(requests);
    }

    /// <summary>
    /// Approve or reject student access request for current teacher
    /// </summary>
    [HttpPut("student-requests/{requestId:guid}/status")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> ReviewStudentRequest(Guid requestId, [FromBody] ReviewStudentConnectionStatusRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var teacherUserId))
        {
            return Unauthorized(new { error = "Invalid user token" });
        }

        if (!Enum.IsDefined(typeof(StudentTeacherConnectionStatus), request.Status))
        {
            return BadRequest(new { error = "Invalid status value" });
        }

        var nextStatus = (StudentTeacherConnectionStatus)request.Status;
        if (nextStatus != StudentTeacherConnectionStatus.Approved && nextStatus != StudentTeacherConnectionStatus.Rejected)
        {
            return BadRequest(new { error = "Only Approved or Rejected is allowed" });
        }

        var connection = await _context.StudentTeacherConnections
            .FirstOrDefaultAsync(x => x.Id == requestId && x.TeacherUserId == teacherUserId, cancellationToken);

        if (connection == null)
        {
            return NotFound(new { error = "Student request not found" });
        }

        connection.Status = nextStatus;
        connection.ReviewedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            requestId = connection.Id,
            status = connection.Status.ToString(),
            reviewedAt = connection.ReviewedAt
        });
    }
}
