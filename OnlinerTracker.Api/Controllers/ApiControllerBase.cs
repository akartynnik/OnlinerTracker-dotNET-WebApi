using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    public class ApiControllerBase : ApiController
    {
        private readonly IPrincipalService _principalService;
        public virtual new Principal User => _principalService.GetSessionUser();

        public ApiControllerBase()
        {
        }

        public ApiControllerBase(IPrincipalService principalService)
        {
            _principalService = principalService;
        }

        public IHttpActionResult Successful()
        {
            return Ok("OK");
        }

        public IHttpActionResult Duplicate()
        {
            return Ok("Duplicate");
        }
    }
}