using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System.Web;

namespace OnlinerTracker.Services
{
    public class PrincipalService : IPrincipalService
    {
        private HttpContext _context;
        public PrincipalService(HttpContext context)
        {
            _context = context;
        }

        public Principal GetSessionUser()
        {
            return _context.User as Principal;
        }
    }
}
