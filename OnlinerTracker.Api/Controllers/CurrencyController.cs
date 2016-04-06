using Autofac.Integration.WebApi;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [AutofacControllerConfiguration]
    [RoutePrefix("api/Currency")]
    public class CurrencyController : ApiControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [Route("Get", Name = "Get currency by type")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(CurrencyType currencyType)
        {
            return Ok(_currencyService.GetCurrent(currencyType));
        }
    }
}