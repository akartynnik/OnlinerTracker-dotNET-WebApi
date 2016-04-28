using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Services.Configs;
using OnlinerTracker.Services.Tests.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace OnlinerTracker.Services.Tests.Unit
{
    public class TrackingServiceTests
    {
        [SetUp]
        public void SetUp()
        {
            SystemThread.SetSleepTime(0);
            SystemTime.Set(new DateTime(2016, 4, 20, 15, 0, 0));
            ServicesFactory.SetCurrentHttpContext();

        }

        [Test]
        public async Task CheckProducts_IfAnyProductHaveNotActualCost_ShouldCallLogService()
        {
            TrackingServiceConfig fakeConfig;
            var service = ServicesFactory.GetTrackingService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            fakeConfig.ProductService.GetCurrentProductCost(Arg.Any<Guid>()).Returns(15000);
            fakeConfig.ExternalProductService.ConvertJsonToProducts(Arg.Any<string>())
                .Returns(new List<Product> {new Product {CurrentCost = 20000}});

            await service.CheckProducts();

            mockLogService.Received()
                .AddJobLog(Arg.Any<JobType>(), Arg.Is<string>(u => u.Contains("Number of updated costs")));
        }

        [Test]
        public async Task CheckProducts_IfAnyProductHaveNotActualCost_ShouldUpdateCostForThisProduct()
        {
            TrackingServiceConfig fakeConfig;
            var service = ServicesFactory.GetTrackingService(out fakeConfig);
            IProductService mockProductService = fakeConfig.ProductService;
            mockProductService.GetAllTracking().Returns(new List<Product>
            {
                new Product {Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a")},
                new Product {Id = Guid.Parse("bba6525f-a3fc-e511-bc42-50e549baaf9c")}
            });// 2 prodcts tracked 
            mockProductService.GetCurrentProductCost(Arg.Any<Guid>()).Returns(15000);
            fakeConfig.ExternalProductService.ConvertJsonToProducts(Arg.Any<string>())
                .Returns(new List<Product> {new Product {CurrentCost = 20000}});

            await service.CheckProducts();

            mockProductService.Received(2).InsertCost(Arg.Any<Cost>()); //recived 2 times
        }

        [Test]
        public async Task CheckProducts_IfAllTrackedProductsHaveActualCosts_ShouldNotCallLogService()
        {
            TrackingServiceConfig fakeConfig;
            var service = ServicesFactory.GetTrackingService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            fakeConfig.ProductService.GetCurrentProductCost(Arg.Any<Guid>()).Returns(20000);
            fakeConfig.ExternalProductService.ConvertJsonToProducts(Arg.Any<string>())
                .Returns(new List<Product> {new Product {CurrentCost = 20000}});

            await service.CheckProducts();

            mockLogService.DidNotReceive().AddJobLog(Arg.Any<JobType>(), Arg.Any<string>());
        }

        [Test]
        public async Task CheckProducts_With_MinutesBeforeCheck_Parameter_IfHaveProductsToUpdateButCurrentHttpContextIsNull_ShouldNotCallLogService()
        {
            TrackingServiceConfig fakeConfig;
            TrackingService service = ServicesFactory.GetTrackingService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            var minutesBeforeCheck = 60;
            HttpContext.Current = null;

            await service.CheckProducts(minutesBeforeCheck);

            mockLogService.DidNotReceive().AddJobLog(Arg.Any<JobType>(), Arg.Any<string>());
        }

        [Test]
        public async Task CheckProducts_With_MinutesBeforeCheck_Parameter_IfHaveNotLastSuccessLog_ShouldCallLogService()
        {
            TrackingServiceConfig fakeConfig;
            TrackingService service = ServicesFactory.GetTrackingService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            var minutesBeforeCheck = 60;
            fakeConfig.ProductService.GetCurrentProductCost(Arg.Any<Guid>()).Returns(15000);
            fakeConfig.ExternalProductService.ConvertJsonToProducts(Arg.Any<string>())
                .Returns(new List<Product> { new Product { CurrentCost = 20000 } });
            mockLogService.GetLastSuccessLog(Arg.Any<JobType>()).ReturnsNull();

            await service.CheckProducts(minutesBeforeCheck);

            mockLogService.Received().AddJobLog(Arg.Any<JobType>(), Arg.Any<string>());
        }

        [Test]
        public async Task CheckProducts_With_MinutesBeforeCheck_Parameter_IfTookLessThenHourFromLastSuccessLog_ShouldNotCallLogService()
        {
            TrackingServiceConfig fakeConfig;
            TrackingService service = ServicesFactory.GetTrackingService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            var minutesBeforeCheck = 60;
            fakeConfig.ProductService.GetCurrentProductCost(Arg.Any<Guid>()).Returns(15000);
            fakeConfig.ExternalProductService.ConvertJsonToProducts(Arg.Any<string>())
                .Returns(new List<Product> { new Product { CurrentCost = 20000 } });
            SystemTime.Set(new DateTime(2016, 4, 19, 15, 30, 0)); //now 20.04.2016 15:00
            mockLogService.GetLastSuccessLog(Arg.Any<JobType>()).Returns(new JobLog
            {
                CheckedAt = new DateTime(2016, 4, 19, 15, 00, 0) //last Success Log Was in 19.04.2016 15:00
            });

            await service.CheckProducts(minutesBeforeCheck);

            mockLogService.DidNotReceive().AddJobLog(Arg.Any<JobType>(), Arg.Any<string>());
        }

        [Test]
        public async Task CheckProducts_With_MinutesBeforeCheck_Parameter_IfTookMoreThenHourFromLastSuccessLog_ShouldCallLogService()
        {
            TrackingServiceConfig fakeConfig;
            TrackingService service = ServicesFactory.GetTrackingService(out fakeConfig);
            ILogService mockLogService = fakeConfig.LogService;
            var minutesBeforeCheck = 60;
            fakeConfig.ProductService.GetCurrentProductCost(Arg.Any<Guid>()).Returns(15000);
            fakeConfig.ExternalProductService.ConvertJsonToProducts(Arg.Any<string>())
                .Returns(new List<Product> { new Product { CurrentCost = 20000 } });
            SystemTime.Set(new DateTime(2016, 4, 19, 16, 30, 0)); //now 20.04.2016 15:00
            mockLogService.GetLastSuccessLog(Arg.Any<JobType>()).Returns(new JobLog
            {
                CheckedAt = new DateTime(2016, 4, 19, 15, 00, 0) //last Success Log Was in 19.04.2016 15:00
            });

            await service.CheckProducts(minutesBeforeCheck);

            mockLogService.Received().AddJobLog(Arg.Any<JobType>(), Arg.Any<string>());
        }

        [TearDown]
        public void TearDown()
        {
            SystemThread.ResetSleepTime();
            SystemTime.Reset();
            ServicesFactory.ResetCurrentHttpContext();
        }
    }
}