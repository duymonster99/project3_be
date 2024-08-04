using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using AuthenticationServices.Models;
using AuthenticationServices.DTOs;

namespace AuthenticationServices.Services
{
    public class EmailServices
    {
        private readonly EmailSettings emailSettings;
        public EmailServices(IOptions<EmailSettings> _emailSettings)
        {
            emailSettings = _emailSettings.Value;
        }
        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            var fromAddress = new MailAddress(emailSettings.FromMail);
            var toAddress = new MailAddress(emailRequest.ToMail);
            var smtp = new SmtpClient
            {
                Host = emailSettings.Host,
                Port = emailSettings.Port,
                EnableSsl = emailSettings.EnableSsl,
                //Network chi ra mail se duoc gui thong qua internet
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //Thong tin xac thuc duoc su dung de ket noi voi may chu SMTP
                Credentials = new NetworkCredential(emailSettings.FromMail, emailSettings.Password)

            };
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = emailRequest.Subject,
                Body = emailRequest.HtmlContent,
                IsBodyHtml = true
            };
            await smtp.SendMailAsync(message);
        }
    }
}
