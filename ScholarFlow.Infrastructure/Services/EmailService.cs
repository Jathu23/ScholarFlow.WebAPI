using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ScholarFlow.Application.Common.Interfaces;
using ScholarFlow.Infrastructure.Settings;

namespace ScholarFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtp;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger)
    {
        _smtp = smtpSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email} with subject '{Subject}'", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName, string role)
    {
        var subject = "Welcome to ScholarFlow!";
        var displayRole = role.Equals("Teacher", StringComparison.OrdinalIgnoreCase)
            ? "Teacher"
            : "Student";

        var body = role.Equals("Teacher", StringComparison.OrdinalIgnoreCase)
            ? BuildTeacherWelcomeHtml(fullName)
            : BuildStudentWelcomeHtml(fullName);

        await SendEmailAsync(toEmail, fullName, subject, body);
    }

    private static string BuildStudentWelcomeHtml(string fullName) => $"""
        <!DOCTYPE html>
        <html>
        <body style="font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 24px;">
          <h2 style="color: #4F46E5;">Welcome to ScholarFlow, {fullName}!</h2>
          <p>Your student account has been created successfully. You can now log in and start exploring papers and exam sessions.</p>
          <p>If you have any questions, feel free to reach out to us.</p>
          <br/>
          <p style="color: #888; font-size: 12px;">— The ScholarFlow Team</p>
        </body>
        </html>
        """;

    private static string BuildTeacherWelcomeHtml(string fullName) => $"""
        <!DOCTYPE html>
        <html>
        <body style="font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 24px;">
          <h2 style="color: #4F46E5;">Welcome to ScholarFlow, {fullName}!</h2>
          <p>Your teacher registration has been received and is currently <strong>pending admin review</strong>.</p>
          <p>You will receive another email once your account has been reviewed. This usually takes 1–2 business days.</p>
          <br/>
          <p style="color: #888; font-size: 12px;">— The ScholarFlow Team</p>
        </body>
        </html>
        """;
}
