using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using TheBlogProject.ViewModels;

namespace TheBlogProject.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_configuration["MailSettings:Mail"]);
            email.To.Add(MailboxAddress.Parse(_configuration["MailSettings:Mail"]));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["MailSettings:Host"], Int32.Parse(_configuration["MailSettings:Port"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["MailSettings:Mail"], _configuration["MailSettings:Password"]);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_configuration["MailSettings:Mail"]);
            email.To.Add(MailboxAddress.Parse(emailTo));
            email.Subject = subject;

            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["MailSettings:Host"], Int32.Parse(_configuration["MailSettings:Port"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["MailSettings:Mail"], _configuration["MailSettings:Password"]);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);

        }
    }
}
