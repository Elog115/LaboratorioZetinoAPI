using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ProyectoZetino.WebMVC.Configuration;
using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string bodyHtml,
            byte[] attachmentBytes,
            string attachmentFileName,
            string attachmentContentType = "application/pdf")
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.FromName, _settings.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = bodyHtml
            };

            if (attachmentBytes != null && attachmentBytes.Length > 0)
            {
                builder.Attachments.Add(
                    attachmentFileName,
                    attachmentBytes,
                    ContentType.Parse(attachmentContentType)
                );
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _settings.SmtpServer,
                _settings.SmtpPort,
                _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto
            );

            await client.AuthenticateAsync(_settings.UserName, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
