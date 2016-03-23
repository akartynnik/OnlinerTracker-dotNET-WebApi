using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlinerTracker.Security
{
    public class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public SecurityDbContext()
            : base("SecurityConnection", throwIfV1Schema: false)
        {
        }

        public static SecurityDbContext Create()
        {
            return new SecurityDbContext();
        }
    }
}
