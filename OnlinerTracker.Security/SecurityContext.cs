using Microsoft.AspNet.Identity.EntityFramework;
using OnlinerTracker.Data.SecurityModels;
using System.Data.Entity;

namespace OnlinerTracker.Security
{
    public class SecurityContext : IdentityDbContext<IdentityUser>
    {
        public SecurityContext()
            : base("SecurityConnection")
        {
            Database.SetInitializer(new SecurityInitializer());
        }
        public DbSet<Client> Clients { get; set; }
    }
}
