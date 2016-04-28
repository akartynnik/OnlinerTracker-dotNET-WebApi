using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Services.Configs;
using System;
using System.Linq;
using System.Web;

namespace OnlinerTracker.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotificationServiceConfig _config;

        public NotificationService(NotificationServiceConfig config)
        {
            _config = config;
        }

        public void SendNotifications(int hourInWhichSendingStart)
        {
            if(HttpContext.Current == null)
                return;
            var lastSuccessLog = _config.LogService.GetLastSuccessLog(JobType.EmailSend);
            if (((lastSuccessLog != null &&
                  lastSuccessLog.CheckedAt.ToString("yy-MM-dd") != SystemTime.Now.ToString("yy-MM-dd"))
                 || (lastSuccessLog == null))
                && SystemTime.Now.Hour == hourInWhichSendingStart)
            {
                SendNotifications();
            }
        }

        public void SendNotifications()
        {
            try
            {
                var usersWithUpdateCount = 0;
                foreach (var user in _config.SecurityRepository.GetAllUsers())
                {
                    var products = _config.ProductService.GetAllChanges(Guid.Parse(user.Id));
                    if (products.Any())
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            _config.MessageSender.SendEmail("Updates for you",
                                _config.StringComposer.GetUserNotificationHtmlString(products), user.Email);
                            usersWithUpdateCount++;
                        }
                    }
                }
                if (usersWithUpdateCount > 0)
                {
                    _config.LogService.AddJobLog(JobType.EmailSend,
                        string.Format("Users number, who gets updates: {0}", usersWithUpdateCount));
                }
            }
            catch (Exception ex)
            {
                _config.LogService.AddJobLog(JobType.EmailSend, ex.Message, false);
            }
        }
    }
}