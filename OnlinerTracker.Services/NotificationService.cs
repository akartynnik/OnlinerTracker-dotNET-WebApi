using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OnlinerTracker.Services
{
    public class NotificationService : INotificationService
    {
        #region Fields and Properties

        private readonly string HtmlTemplite = "<html><body>{0}</body></html>";

        private readonly SmtpClient _defaultServiceClient;

        private readonly SecurityRepository _securityRepo;

        private readonly IProductService _productService;

        private readonly ILogService _logService;

        private readonly string _smtpAccount;

        public string EmailSenderName { get; set; }

        public string HourInWhichSendingStart { get; set; }

        #endregion

        #region Constructor

        public NotificationService(IProductService productService, ILogService logService, string smtpHost, 
            string smtpPortString, string smtpAccount, string smtpPassword)
        {
            int smtpPort;
            int.TryParse(smtpPortString, out smtpPort);
            _defaultServiceClient = new SmtpClient
            {
                Host = smtpHost,
                Port = smtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpAccount, smtpPassword)
            };
            _securityRepo = new SecurityRepository();
            _smtpAccount = smtpAccount;
            _productService = productService;
            _logService = logService;
        }

        #endregion

        #region INotificationService methods

        public async Task<string> CheckNotifications()
        {
            var hourInWhichSendingStart = 0;
            int.TryParse(HourInWhichSendingStart, out hourInWhichSendingStart);
            var lastSuccessLog = _logService.GetLastSuccessLog(JobType.EmailSend);
            if ((lastSuccessLog != null && lastSuccessLog.CheckedAt.Hour == hourInWhichSendingStart) || DateTime.Now.Hour != hourInWhichSendingStart)
                return string.Empty;
            StartSend();
            return string.Empty;
        }

        #endregion

        #region Additional methods

        public void StartSend()
        {
            try
            {
                var usersWithUpdateCount = 0;
                foreach (var user in _securityRepo.GetAllUsers())
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        var products = _productService.GetAllChanges(Guid.Parse(user.Id));
                        if (products.Any())
                        {
                            EmailSend("Updates for you", NotificationBodyGenerator(products), user.Email);
                            usersWithUpdateCount++;
                        }
                    }
                }
                _logService.AddJobLog(JobType.EmailSend, string.Format("Users number, who gets updates: {0}", usersWithUpdateCount));
            }
            catch (Exception ex)
            {
                _logService.AddJobLog(JobType.EmailSend, ex.Message, false);
            }
        }

        public void EmailSend(string subject, string body, string recipientEmail)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_smtpAccount, EmailSenderName),
                Subject = subject,
                Body = string.Format(HtmlTemplite, body),
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(recipientEmail));
            message.ReplyToList.Add(new MailAddress(_smtpAccount, EmailSenderName));
            _defaultServiceClient.SendAsync(message, null);
        }

        public string NotificationBodyGenerator(IEnumerable<ProductForNotification> products)
        {
            return products.Aggregate(string.Empty,
                (current, product) =>
                    current +
                    (product.Name + " <b>" + product.CurrentCost + "</b> ( old:" + product.DayAgoCost + ")<br/>"));
        }

        #endregion
    }
}