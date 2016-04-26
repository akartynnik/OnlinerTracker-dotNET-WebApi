using NUnit.Framework;
using OnlinerTracker.Data;
using OnlinerTracker.Services.BaseServices;
using System.Collections.Generic;

namespace OnlinerTracker.Services.Tests.Unit.BaseServices
{
    [TestFixture]
    public class StringComposerTests
    {
        [Test]
        public void GetUserNotificationHtmlString_Contains_ProductNameAndProductCurrentCostAndProductDayAgoCost()
        {
            var productsList = new List<ProductForNotification>
            {
                new ProductForNotification
                {
                    Name = "ProductName",
                    CurrentCost = 20000,
                    DayAgoCost = 15000

                }
            };
            var service = new StringComposer();

            var result = service.GetUserNotificationHtmlString(productsList);

            StringAssert.Contains("ProductName", result);
            StringAssert.Contains("20000", result);
            StringAssert.Contains("15000", result);
        }

        [Test]
        public void GetUserNotificationHtmlString_ResultInHtmlFormat_ShouldContainsHtmlTag()
        {
            var productsList = new List<ProductForNotification>();
            var service = new StringComposer();

            var result = service.GetUserNotificationHtmlString(productsList);

            StringAssert.Contains("<html>", result);
        }
    }
}
