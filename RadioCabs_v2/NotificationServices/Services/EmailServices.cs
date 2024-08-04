using Microsoft.Extensions.Options;
using NotificationServices.Model;
using System.Net;
using System.Net.Mail;
using NotificationServices.ConfigSetting;

namespace NotificationServices.Services;

public class EmailServices
{
    private readonly EmailSettings _emailSettings;

    public EmailServices(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(EmailRequest emailRequest)
    {
        var fromAddress = new MailAddress(_emailSettings.FromMail);
        var toAddress = new MailAddress(emailRequest.ToMail);

        var smtp = new SmtpClient
        {
            Host = _emailSettings.Host,
            Port = _emailSettings.Port,
            EnableSsl = _emailSettings.EnableSSL,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(fromAddress.Address, _emailSettings.Password),
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