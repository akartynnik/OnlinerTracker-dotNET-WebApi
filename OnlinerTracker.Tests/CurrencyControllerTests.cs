using NSubstitute;
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
            var fakeCurrancyService = Substitute.For<ICurrencyService>();
            var fakePrincipleService = Substitute.For<IPrincipalService>();
            fakeCurrancyService.GetCurrent(CurrencyType.BLR).ReturnsForAnyArgs(new Currency());
            var controller = new CurrencyController(fakeCurrancyService, fakePrincipleService);

            var result = await controller.Get(CurrencyType.USD) as OkNegotiatedContentResult<Currency>;

            Assert.IsNotNull(result);
        }
    }
}
