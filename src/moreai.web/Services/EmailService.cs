using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using moreai.web.Configurations;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace moreai.web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            try
            {
                if (_emailSettings.Provider?.ToLower() == "sendgrid")
                {
                    await SendViaGridAsync(to, subject, body, isHtml);
                }
                else
                {
                    await SendViaSmtpAsync(to, subject, body, isHtml);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送郵件時發生錯誤。收件者: {To}, 主旨: {Subject}", to, subject);
                throw;
            }
        }

        private async Task SendViaGridAsync(string to, string subject, string body, bool isHtml)
        {
            var client = new SendGridClient(_emailSettings.SendGridApiKey);
            var from = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, 
                !isHtml ? body : null, isHtml ? body : null);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"SendGrid 發送失敗: {response.StatusCode}");
            }
        }

        private async Task SendViaSmtpAsync(string to, string subject, string body, bool isHtml)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            message.To.Add(to);

            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                EnableSsl = _emailSettings.SmtpUseSsl
            };

            await client.SendMailAsync(message);
        }

        public async Task SendWelcomeEmailAsync(string to, string username)
        {
            var subject = "歡迎加入 MoreAI";
            var body = $@"
                <h1>歡迎 {username}！</h1>
                <p>感謝您註冊成為 MoreAI 的會員。</p>
                <p>如有任何問題，請隨時聯繫我們的客服團隊。</p>
                <br>
                <p>祝您使用愉快！</p>
                <p>MoreAI 團隊</p>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetToken)
        {
            var subject = "MoreAI 密碼重設";
            var resetLink = $"https://moreai.com/reset-password?token={resetToken}";
            var body = $@"
                <h2>密碼重設請求</h2>
                <p>我們收到了您的密碼重設請求。</p>
                <p>請點擊下面的連結來重設您的密碼：</p>
                <p><a href='{resetLink}'>{resetLink}</a></p>
                <p>如果您沒有要求重設密碼，請忽略此郵件。</p>
                <br>
                <p>謝謝</p>
                <p>MoreAI 團隊</p>";

            await SendEmailAsync(to, subject, body, true);
        }
    }
}