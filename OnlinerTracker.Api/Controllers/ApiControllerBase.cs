using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    public abstract class ApiControllerBase : ApiController
    {
        private readonly IPrincipalService _principalService;
        public virtual new Principal User => _principalService.GetSessionUser();

        protected ApiControllerBase()
        {
        }

        protected ApiControllerBase(IPrincipalService principalService)
        {
            _principalService = principalService;
        }

        protected IHttpActionResult Successful()
        {
            return Ok("OK");
        }

        protected IHttpActionResult Duplicate()
        {
            return Ok("Duplicate");
        }
    }
}