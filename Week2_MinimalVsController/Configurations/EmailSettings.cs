namespace Week2_MinimalVsController.Configurations;

public class EmailSettings
{
    // appsettings.json içindeki anahtar isimleriyle birebir aynı olmalı
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
}