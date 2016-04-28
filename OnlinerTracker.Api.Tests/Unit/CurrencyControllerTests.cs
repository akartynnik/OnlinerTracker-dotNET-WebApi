using NSubstitute;
using NUnit.Framework;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Api.Tests.Base;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace OnlinerTracker.Api.Tests.Unit
{
    [TestFixture]
    public class CurrencyControllerTests : TestsClassBase
    {
        [Test]
        public async Task Get_Return_OkHttpStatusWithCurrencyValueLikeCurrencyValueFromCurerncyServiceResponse()
        {
           
            ICurrencyService stubCurrancyService;
            CurrencyController testedController = ControllersFactory.GetCurrencyController(out stubCurrancyService);
            stubCurrancyService.GetCurrent(Arg.Any<CurrencyType>()).Returns(DefaultCurrency);

            var result = await testedController.Get(CurrencyType.USD) as OkNegotiatedContentResult<Currency>;

            Assert.IsNotNull(result);
            Assert.AreEqual(DefaultCurrency.Value, result.Content.Value);
        }
    }
}
