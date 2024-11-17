using Kemet.Core.Entities;
using Kemet.Core.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kemet.Services
{
    public class EmailSettings : IEmailSettings
    {
        private readonly IOptions<MailSettings> _options;

        public EmailSettings(IOptions<MailSettings> options)
        {
            _options = options;
        }

        public async Task SendEmailAsync(Email email)
        {
            var mail = new MimeMessage();

            try
            {
                mail.Sender = MailboxAddress.Parse(_options.Value.Email);
                mail.Subject = email.Subject;
                mail.To.Add(MailboxAddress.Parse(email.Recipients));

                // Optional: Set From address with display name

                mail.From.Add(new MailboxAddress(_options.Value.DisplayName, _options.Value.Email));


                var bodyPart = new TextPart("plain") { Text = email.Body };

                mail.Body = bodyPart;

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_options.Value.Host, _options.Value.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_options.Value.Email, _options.Value.Password);
                await smtp.SendAsync(mail);
            }
            catch (Exception ex)
            {
                // Handle email sending errors (e.g., log the exception)
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
