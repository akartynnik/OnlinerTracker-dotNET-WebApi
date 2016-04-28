using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System.Net;
using System.Net.Mail;

namespace OnlinerTracker.Services.BaseServices
{
    public class MessageSender : IMessageSender
    {
        private readonly MessageSenderConfig _config;

        public MessageSender(MessageSenderConfig config)
        {
            _config = config;
        }

        public void SendEmail(string subject, string body, string recipientEmail, bool isBodyHtml = true)
        {
            var smtpPort= int.Parse(_config.SmtpPortString);
            var smtpClient = new SmtpClient
            {
                Port = smtpPort,
                Host = _config.SmtpHost,
                EnableSsl = true,
                Credentials = new NetworkCredential(_config.SmtpAccount, _config.SmtpPassword)
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config.SmtpAccount, _config.EmailSenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };
            message.To.Add(new MailAddress(recipientEmail));
            message.ReplyToList.Add(new MailAddress(_config.SmtpAccount, _config.EmailSenderName));
            smtpClient.SendAsync(message, null);
        }
        
    }
}
