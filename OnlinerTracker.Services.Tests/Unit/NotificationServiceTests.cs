using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using OnlinerTracker.Services.Configs;
using OnlinerTracker.Services.Tests.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace OnlinerTracker.Services.Tests.Unit
{
    [TestFixture]
    public class NotificationServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://test.test", ""),
                new HttpResponse(new StringWriter())
            );
        }

        [Test]
        public void SendNotifications_With_HourInWhichSendingStart_Parameter_IfNowHourInWhichSendingShouldStartAndLastSuccessLogWasNotEqualsNowaday_ShouldCallMessageSender()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            IMessageSender mockMessageSender = fakeConfig.MessageSender;
            var hourInWhichSendingStart = 15;
            SystemTime.Set(new DateTime(2016,4,20,15,0,0)); //now 20.04.2016 15:00
            fakeConfig.LogService.GetLastSuccessLog(Arg.Any<JobType>()).Returns(new JobLog
            {
                CheckedAt = new DateTime(2016, 4, 19, 15, 00, 0) //last Success Log Was in 19.04.2016 15:00
            });

            notificationService.SendNotifications(hourInWhichSendingStart);

            mockMessageSender.Received().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifications_With_HourInWhichSendingStart_Parameter__IfNowHourInWhichSendingShouldStartAndHaveNotLastSuccessLog_ShouldCallMessageSender()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            IMessageSender mockMessageSender = fakeConfig.MessageSender;
            var hourInWhichSendingStart = 16;
            SystemTime.Set(new DateTime(2016, 4, 20, 16, 30, 0));
            fakeConfig.LogService.GetLastSuccessLog(Arg.Any<JobType>()).ReturnsNull();

            notificationService.SendNotifications(hourInWhichSendingStart);

            mockMessageSender.Received().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifications_With_HourInWhichSendingStart_Parameter_IfCurrentHttpContextIsNull_ShouldNotCallMessageSender()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            IMessageSender mockMessageSender = fakeConfig.MessageSender;
            var hourInWhichSendingStart = 16;
            SystemTime.Set(new DateTime(2016, 4, 20, 16, 30, 0));
            fakeConfig.LogService.GetLastSuccessLog(Arg.Any<JobType>()).ReturnsNull();
            HttpContext.Current = null;

            notificationService.SendNotifications(hourInWhichSendingStart);

            mockMessageSender.DidNotReceive().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifications_IfHaveNotNotificationUsers_ShouldNotCallMessageSender()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            IMessageSender mockMessageSender = fakeConfig.MessageSender;
            fakeConfig.SecurityRepository.GetAllUsers().ReturnsNull();

            notificationService.SendNotifications();

            mockMessageSender.DidNotReceive().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifications_IfNotificationUsersHaveNotEmails_ShouldNotCallMessageSender()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            IMessageSender mockMessageSender = fakeConfig.MessageSender;
            fakeConfig.SecurityRepository.GetAllUsers().Returns(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "FirstUser",
                    Email = string.Empty
                },
                 new ApplicationUser
                {
                    UserName = "SecondUser",
                    Email = string.Empty
                }
            });

            notificationService.SendNotifications();

            mockMessageSender.DidNotReceive().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifications_IfNotificationUsersHaveNotAnyProducts_ShouldNotCallMessageSender()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            IMessageSender mockMessageSender = fakeConfig.MessageSender;
            fakeConfig.ProductService.GetAllChanges(Arg.Any<Guid>()).ReturnsNull();

            notificationService.SendNotifications();

            mockMessageSender.DidNotReceive().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifications_IfNotificationUsersHaveEmailsAndProducts_ShouldCallLogService()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            fakeConfig.SecurityRepository.GetAllUsers().Returns(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = "need@for.test"
                }
            });
            fakeConfig.ProductService.GetAllChanges(Arg.Any<Guid>()).Returns(new List<ProductForNotification> { new ProductForNotification() });

            notificationService.SendNotifications();

            mockLogService.Received().AddJobLog(Arg.Is<JobType>(u => u == JobType.EmailSend), Arg.Is<string>(u => u.Contains("Users number, who gets updates:")));
        }

        [Test]
        public void SendNotifications_IfThrowError_ShouldCallLogService()
        {
            NotificationServiceConfig fakeConfig;
            NotificationService notificationService = ServicesFactory.GetNotificationService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            fakeConfig.SecurityRepository.GetAllUsers().Throws(new Exception("Test exception catched"));

            notificationService.SendNotifications();

            mockLogService.Received().AddJobLog(Arg.Is<JobType>(u => u == JobType.EmailSend), Arg.Is<string>(u => u.Contains("Test exception catched")), Arg.Any<bool>());
        }

        [TearDown]
        public void TearDown()
        {
            SystemTime.Reset();
            HttpContext.Current = null;
        }
    }
}
