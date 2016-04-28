using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using OnlinerTracker.Services.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace OnlinerTracker.Services.Tests.Base
{
    public class ServicesFactory
    {
        public static NotificationService GetNotificationService(out NotificationServiceConfig fakeConfig)
        {
            IProductService fakeProductService = Substitute.For<IProductService>();
            ISecurityRepository fakeSecurityRepository = Substitute.For<ISecurityRepository>();
            ILogService fakeLogService = Substitute.For<ILogService>();
            IMessageSender fakeMessageSender = Substitute.For<IMessageSender>();
            IStringComposer fakeStringComposer = Substitute.For<IStringComposer>();
            fakeSecurityRepository.GetAllUsers().Returns(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Email = "need@for.test"
                }
            });
            fakeProductService.GetAllChanges(Arg.Any<Guid>()).Returns(new List<ProductForNotification> { new ProductForNotification() });
            fakeLogService.GetLastSuccessLog(Arg.Any<JobType>()).ReturnsNull();
            fakeConfig = new NotificationServiceConfig
            {
                ProductService = fakeProductService,
                SecurityRepository = fakeSecurityRepository,
                LogService = fakeLogService,
                StringComposer = fakeStringComposer,
                MessageSender = fakeMessageSender
            };

            return new NotificationService(fakeConfig);
        }

        public static TrackingService GetTrackingService(out TrackingServiceConfig fakeConfig)
        {
            IProductService fakeProductService = Substitute.For<IProductService>();
            IExternalProductService fakeExternalProductService = Substitute.For<IExternalProductService>();
            ILogService fakeLogService = Substitute.For<ILogService>();
            fakeProductService.GetAllTracking().Returns(new List<Product>
            {
                new Product {Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a")},
                new Product {Id = Guid.Parse("bba6525f-a3fc-e511-bc42-50e549baaf9c")}
            });
            fakeConfig = new TrackingServiceConfig
            {
                ProductService = fakeProductService,
                LogService = fakeLogService,
                ExternalProductService = fakeExternalProductService
            };

            return new TrackingService(fakeConfig);
        }

        public static void SetCurrentHttpContext()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://test.test", ""),
                new HttpResponse(new StringWriter())
            );
        }

        public static void ResetCurrentHttpContext()
        {
            HttpContext.Current = null;
        }
    }
}
