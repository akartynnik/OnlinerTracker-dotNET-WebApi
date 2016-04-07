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

        private readonly SecurityRepository _securityRepository;

        private readonly IProductService _productService;

        private readonly ILogService _logService;

        private readonly NotificationConfig _notificationConfig;

        public string EmailSenderName { get; set; }

        public string HourInWhichSendingStart { get; set; }

        #endregion

        #region Constructor

        public NotificationService(IProductService productService, ILogService logService,
            SecurityRepository securityRepository, NotificationConfig notificationConfig)
        {
            int smtpPort;
            int.TryParse(notificationConfig.SmtpPortString, out smtpPort);
            _defaultServiceClient = new SmtpClient
            {
                Host = notificationConfig.SmtpHost,
                Port = smtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(notificationConfig.SmtpAccount, notificationConfig.SmtpPassword)
            };
            _securityRepository = securityRepository;
            _notificationConfig = notificationConfig;
            _productService = productService;
            _logService = logService;
        }

        #endregion

        #region INotificationService methods

        public async Task CheckNotifications()
        {
            var hourInWhichSendingStart = 0;
            if (int.TryParse(HourInWhichSendingStart, out hourInWhichSendingStart))
            {
                var lastSuccessLog = _logService.GetLastSuccessLog(JobType.EmailSend);
                if ((lastSuccessLog != null && lastSuccessLog.CheckedAt.Hour == hourInWhichSendingStart) ||
                    DateTime.Now.Hour != hourInWhichSendingStart)
                {
                    StartSend();
                }
            }
        }

        #endregion

        #region Additional methods

        private void StartSend()
        {
            try
            {
                var usersWithUpdateCount = 0;
                foreach (var user in _securityRepository.GetAllUsers())
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
                if(usersWithUpdateCount > 0)
                {
                    _logService.AddJobLog(JobType.EmailSend, string.Format("Users number, who gets updates: {0}", usersWithUpdateCount));
                }
            }
            catch (Exception ex)
            {
                _logService.AddJobLog(JobType.EmailSend, ex.Message, false);
            }
        }

        private void EmailSend(string subject, string body, string recipientEmail)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_notificationConfig.SmtpAccount, EmailSenderName),
                Subject = subject,
                Body = string.Format(HtmlTemplite, body),
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(recipientEmail));
            message.ReplyToList.Add(new MailAddress(_notificationConfig.SmtpAccount, EmailSenderName));
            _defaultServiceClient.SendAsync(message, null);
        }

        private string NotificationBodyGenerator(IEnumerable<ProductForNotification> products)
        {
            return products.Aggregate(string.Empty,
                (current, product) =>
                    current +
                    (product.Name + " <b>" + product.CurrentCost + "</b> ( old:" + product.DayAgoCost + ")<br/>"));
        }

        #endregion
    }
}