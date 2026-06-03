namespace InventoryDashboard.Infrastructure.Options;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string FromName { get; set; } = "Inventory Dashboard";
    public string FromEmail { get; set; } = string.Empty;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPass { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}
