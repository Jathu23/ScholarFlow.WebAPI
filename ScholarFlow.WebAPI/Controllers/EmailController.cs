using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarFlow.Application.Common.Interfaces;

namespace ScholarFlow.WebAPI.Controllers;

/// <summary>
/// Email test endpoint (Admin only)
/// </summary>
[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public class SendTestEmailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string ToName { get; set; } = "Test User";
        public string Subject { get; set; } = "Test Email from ScholarFlow";
        public string Body { get; set; } = "<h2>Hello!</h2><p>This is a test email from ScholarFlow.</p>";
    }

    /// <summary>
    /// Send a test email — Admin only
    /// </summary>
    [HttpPost("test")]
    public async Task<IActionResult> SendTestEmail([FromBody] SendTestEmailRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.ToEmail))
            return BadRequest(new { error = "ToEmail is required" });

        try
        {
            await _emailService.SendEmailAsync(request.ToEmail, request.ToName, request.Subject, request.Body);
            return Ok(new { message = $"Email sent to {request.ToEmail}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to send email", detail = ex.Message });
        }
    }
}
