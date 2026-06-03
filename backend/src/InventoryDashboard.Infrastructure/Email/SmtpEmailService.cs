using InventoryDashboard.Application.Interfaces;
using InventoryDashboard.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InventoryDashboard.Infrastructure.Email;

public class SmtpEmailService(
    IOptions<EmailSettings> emailOptions,
    IConfiguration configuration,
    ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public async Task SendPasswordResetAsync(
        string toEmail,
        string recipientName,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.FromEmail)
            || string.IsNullOrWhiteSpace(_settings.SmtpUser)
            || string.IsNullOrWhiteSpace(_settings.SmtpPass))
        {
            throw new InvalidOperationException("Email is not configured. Set EmailSettings in appsettings.");
        }

        var frontendBase = configuration["Frontend:BaseUrl"]?.TrimEnd('/')
            ?? "http://localhost:5173";
        var resetLink = $"{frontendBase}/login?reset={Uri.EscapeDataString(resetToken)}";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Reset your Inventory Dashboard password";

        var body = new BodyBuilder
        {
            HtmlBody = $"""
                <p>Hi {System.Net.WebUtility.HtmlEncode(recipientName)},</p>
                <p>We received a request to reset your password. Click the link below (valid for 1 hour):</p>
                <p><a href="{resetLink}">Reset password</a></p>
                <p>Or copy this token into the reset form:</p>
                <p style="font-family:monospace;word-break:break-all;">{System.Net.WebUtility.HtmlEncode(resetToken)}</p>
                <p>If you did not request this, you can ignore this email.</p>
                """
        };
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _settings.SmtpServer,
            _settings.Port,
            _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto,
            cancellationToken);
        await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Password reset email sent to {Email}", toEmail);
    }
}
