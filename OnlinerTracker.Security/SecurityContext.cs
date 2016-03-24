using Microsoft.AspNet.Identity.EntityFramework;
using OnlinerTracker.Data;
using System.Data.Entity;

namespace OnlinerTracker.Security
{
    public class SecurityContext : IdentityDbContext<IdentityUser>
    {
        public SecurityContext()
            : base("SecurityConnection")
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
