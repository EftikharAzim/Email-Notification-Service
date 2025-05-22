using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Options;
using MvcWebApp.Services;

namespace MvcWebApp.Services
{
    // This class is used by the application to send Email messages
    // For more SmtpClient details, visit https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer) ||
                string.IsNullOrEmpty(_emailSettings.SmtpUser) ||
                string.IsNullOrEmpty(_emailSettings.SmtpPass) ||
                string.IsNullOrEmpty(_emailSettings.SenderEmail))
            {
                // Log or handle missing configuration
                // For this example, we'll skip sending if not configured.
                // In a real app, you might throw an exception or log an error.
                Console.WriteLine("Email settings are not fully configured. Email not sent.");
                return;
            }

            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName ?? "MyApp", _emailSettings.SenderEmail));
                mimeMessage.To.Add(MailboxAddress.Parse(email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

                using (var client = new SmtpClient())
                {
                    // For demo purposes, accept all SSL certificates (in case of self-signed)
                    // DON'T USE THIS IN PRODUCTION
                    // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                    
                    // Note: only needed if the SMTP server requires authentication
                    if (!string.IsNullOrEmpty(_emailSettings.SmtpUser) && !string.IsNullOrEmpty(_emailSettings.SmtpPass))
                    {
                        await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                    }
                    
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
                // In a real app, you'd use a proper logging framework.
                throw; // Re-throw or handle appropriately
            }
        }
    }

    public class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? SmtpUser { get; set; } // Username for SMTP authentication
        public string? SmtpPass { get; set; } // Password for SMTP authentication
        public string? SenderEmail { get; set; } // The 'From' email address
        public string? SenderName { get; set; } // The 'From' display name
    }
}