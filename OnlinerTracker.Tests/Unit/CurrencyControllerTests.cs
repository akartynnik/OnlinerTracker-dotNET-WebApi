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
    public class CurrencyControllerTests : BaseTestsClass
    {
        [Test]
        public async Task Get_ReturnValue_ShouldBeOkHttpStatusWithCurrencyValueLikeCurrencyValueFromCurerncyServiceResponse()
        {
            var fakeCurency = new Currency
            {
                Type = CurrencyType.USD,
                Value = 20000
            };
            var stubPrincipleService = GetStubForPrincipalService();
            var stubCurrancyService = Substitute.For<ICurrencyService>();
            stubCurrancyService.GetCurrent(Arg.Any<CurrencyType>()).Returns(fakeCurency);
            var testedController = new CurrencyController(stubCurrancyService, stubPrincipleService);

            var result = await testedController.Get(CurrencyType.USD) as OkNegotiatedContentResult<Currency>;

            Assert.IsNotNull(result);
            Assert.AreEqual(fakeCurency.Value, result.Content.Value);
        }
    }
}
