using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using OnlinerTracker.Services.Configs;
using System;
using System.Collections.Generic;

namespace OnlinerTracker.Services.Tests.Base
{
    public class ServicesFactory
    {
        public static NotificationService GetNotificationService(out NotificationServiceConfig fakeConfig)
        {
            IProductService stubProductService = Substitute.For<IProductService>();
            ISecurityRepository stubSecurityRepository = Substitute.For<ISecurityRepository>();
            ILogService stubLogService = Substitute.For<ILogService>();
            IMessageSender mockMessageSender = Substitute.For<IMessageSender>();
            IStringComposer stubStringComposer = Substitute.For<IStringComposer>();
            stubSecurityRepository.GetAllUsers().Returns(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = "need@for.test"
                }
            });
            stubProductService.GetAllChanges(Arg.Any<Guid>()).Returns(new List<ProductForNotification> { new ProductForNotification() });
            stubLogService.GetLastSuccessLog(Arg.Any<JobType>()).ReturnsNull();
            fakeConfig = new NotificationServiceConfig
            {
                ProductService = stubProductService,
                SecurityRepository = stubSecurityRepository,
                LogService = stubLogService,
                StringComposer = stubStringComposer,
                MessageSender = mockMessageSender
            };

            return new NotificationService(fakeConfig);
        }
    }
}
