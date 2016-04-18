using NUnit.Framework;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace OnlinerTracker.Api.Tests
{
    [TestFixture]
    public class CurrencyControllerTests
    {
        [Test]
        public async Task Get_IfNotReturnNull_ShouldReturnOkHttpStatusCode()
        {
            var currancyService = new FakeCurrencyService();
            var controller = new CurrencyController(currancyService);

            var result = await controller.Get(CurrencyType.USD) as OkNegotiatedContentResult<Currency>;

            Assert.IsNotNull(result);
        }

        public class FakeCurrencyService : ICurrencyService
        {
            public Currency GetCurrent(CurrencyType currencyType)
            {
                return new Currency();
            }
        }
    }
}
