using OnlinerTracker.Security;
using System.Web;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    public class ApiControllerBase : ApiController
    {
        protected virtual new Principal User
        {
            get { return HttpContext.Current.User as Principal; }
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