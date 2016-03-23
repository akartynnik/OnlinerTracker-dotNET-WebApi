using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlinerTracker.Api
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext()
            : base("AuthContext")
        {

        }
    }
}