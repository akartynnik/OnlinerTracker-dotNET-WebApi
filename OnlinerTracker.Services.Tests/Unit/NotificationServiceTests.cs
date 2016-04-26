using NSubstitute;
using NUnit.Framework;
using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using OnlinerTracker.Services.Configs;
using System;
using System.Collections.Generic;
using NSubstitute.ReturnsExtensions;

namespace OnlinerTracker.Services.Tests.Unit
{
    [TestFixture]
    public class NotificationServiceTests
    {
        [Test]
        public void SendNotifications_IfNowHourInWhichSendingShouldStartAndLastSuccessLogWasNotEqualsNowaday_ShouldCallMessageSender()
        {
            var hourInWhichSendingStart = 15;
            SystemTime.Set(new DateTime(2016,4,20,15,0,0)); //now 20.04.2016 15:00
            DateTime lastSendingDateTime = new DateTime(2016, 4, 19, 15, 00, 0); //last Success Log Was in 19.04.2016 15:00
            IProductService stubProductService = Substitute.For<IProductService>();
            SecurityRepository stubSecurityRepository = Substitute.For<SecurityRepository>();
            ILogService stubLogService = Substitute.For<ILogService>();
            IMessageSender mockMessageSender = Substitute.For<IMessageSender>();
            IStringComposer stubStringComposer = Substitute.For<IStringComposer>();
            var fakeConfig = new NotificationServiceConfig
            {
                ProductService = stubProductService,
                SecurityRepository = stubSecurityRepository,
                LogService = stubLogService,
                StringComposer = stubStringComposer,
                MessageSender = mockMessageSender
            };
            stubLogService.GetLastSuccessLog(Arg.Any<JobType>()).Returns(new JobLog
            {
                CheckedAt = lastSendingDateTime
            });
            stubSecurityRepository.GetAllUsers().Returns(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = "need@for.test"
                }
            });
            stubProductService.GetAllChanges(Arg.Any<Guid>()).Returns(new List<ProductForNotification>{new ProductForNotification()});
            var testService = new NotificationService(fakeConfig);

            testService.SendNotifications(hourInWhichSendingStart);

            mockMessageSender.Received().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());

        }

        [Test]
        public void SendNotifications_IfNowHourInWhichSendingShouldStartAndHaveNotLastSuccessLog_ShouldCallMessageSender()
        {
            var hourInWhichSendingStart = 15;
            SystemTime.Set(new DateTime(2016, 4, 20, 15, 0, 0)); //now 20.04.2016 15:00
            DateTime lastSendingDateTime = new DateTime(2016, 4, 19, 15, 00, 0); //last Success Log Was in 19.04.2016 15:00
            IProductService stubProductService = Substitute.For<IProductService>();
            SecurityRepository stubSecurityRepository = Substitute.For<SecurityRepository>();
            ILogService stubLogService = Substitute.For<ILogService>();
            IMessageSender mockMessageSender = Substitute.For<IMessageSender>();
            IStringComposer stubStringComposer = Substitute.For<IStringComposer>();
            var fakeConfig = new NotificationServiceConfig
            {
                ProductService = stubProductService,
                SecurityRepository = stubSecurityRepository,
                LogService = stubLogService,
                StringComposer = stubStringComposer,
                MessageSender = mockMessageSender
            };
            stubLogService.GetLastSuccessLog(Arg.Any<JobType>()).ReturnsNull(); //defference
            stubSecurityRepository.GetAllUsers().Returns(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = "need@for.test"
                }
            });
            stubProductService.GetAllChanges(Arg.Any<Guid>()).Returns(new List<ProductForNotification> { new ProductForNotification() });
            var testService = new NotificationService(fakeConfig);

            testService.SendNotifications(hourInWhichSendingStart);

            mockMessageSender.Received().SendEmail(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());

        }
    }
}
