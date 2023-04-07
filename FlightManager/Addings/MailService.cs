using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace FlightManager.Addings
{
    public class MailService
    {

    }
    public interface IEmailService
    {
        Task SendEmailAsync(EmailInfo emailInfo);
        Task SendEmailTemplateAsync(EmailSource emailSource);
    }

    public class EmailService : IEmailService, IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> mailSettings)
        {
            _emailSettings = mailSettings.Value;
        }
        public async Task SendEmailAsync(EmailInfo emailInfo)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.EMail);
            email.To.Add(MailboxAddress.Parse(emailInfo.EmailTo));
            email.Subject = emailInfo.Subject;
            var builder = new BodyBuilder();
            if (emailInfo.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in emailInfo.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, MimeKit.ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = emailInfo.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.EMail, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var messsage = new MimeMessage();
            messsage.Sender = MailboxAddress.Parse(_emailSettings.EMail);
            messsage.To.Add(MailboxAddress.Parse(email));
            messsage.Subject = subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            messsage.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.EMail, _emailSettings.Password);
            await smtp.SendAsync(messsage);
            smtp.Disconnect(true);
        }

        public async Task SendEmailTemplateAsync(EmailSource emailSource)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\CustomTemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", emailSource.UserName).Replace("[email]", emailSource.EmailTo);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.EMail);
            email.To.Add(MailboxAddress.Parse(emailSource.EmailTo));
            email.Subject = $"Welcome {emailSource.UserName}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailSettings.EMail, _emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

    }
    public class EmailInfo
    {
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
    public class EmailSource
    {
        public string EmailTo { get; set; }
        public string UserName { get; set; }
    }
}
