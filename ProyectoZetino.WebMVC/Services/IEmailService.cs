using System.Threading.Tasks;

namespace ProyectoZetino.WebMVC.Services
{
    public interface IEmailService
    {
        Task SendEmailWithAttachmentAsync(
            string toEmail,
            string subject,
            string bodyHtml,
            byte[] attachmentBytes,
            string attachmentFileName,
            string attachmentContentType = "application/pdf");
    }
}

