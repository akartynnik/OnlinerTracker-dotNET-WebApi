using FluentScheduler;
using OnlinerTracker.Interfaces;
using System.Configuration;

namespace OnlinerTracker.Api.Jobs
{
    public class NotificationJob: IJob
    {
        private readonly object _lock = new object();

        private readonly INotificationService _notificationService;

        public NotificationJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Execute()
        {
            lock (_lock)
            {
                var hourInWhichSendingStart = int.Parse(ConfigurationManager.AppSettings["schedulerConfig:hourInWhichSendingStart"]);
                _notificationService.SendNotifications(hourInWhichSendingStart);
            }
        }
    }
}