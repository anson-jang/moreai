namespace moreai.web.Configurations
{
    public class EmailSettings
    {
        public string Provider { get; set; } = "Smtp"; // Smtp 或 SendGrid
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
        
        // SendGrid 設定
        public string SendGridApiKey { get; set; } = "";
        
        // SMTP 設定
        public string SmtpServer { get; set; } = "";
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } = "";
        public string SmtpPassword { get; set; } = "";
        public bool SmtpUseSsl { get; set; }
    }
}