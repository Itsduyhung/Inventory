namespace InventoryDashboard.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetAsync(
        string toEmail,
        string recipientName,
        string resetToken,
        CancellationToken cancellationToken = default);
}
