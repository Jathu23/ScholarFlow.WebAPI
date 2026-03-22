namespace ScholarFlow.Application.Common.Interfaces;

/// <summary>
/// Email notification service interface
/// </summary>
public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
    Task SendWelcomeEmailAsync(string toEmail, string fullName, string role);
}
