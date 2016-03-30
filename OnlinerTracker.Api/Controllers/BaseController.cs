using OnlinerTracker.Security;
using System.Web;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    public class BaseController : ApiController
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.Current.User as CustomPrincipal; }
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