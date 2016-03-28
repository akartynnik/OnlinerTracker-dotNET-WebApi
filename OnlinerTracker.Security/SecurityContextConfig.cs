using System.Data.Entity.Migrations;

namespace OnlinerTracker.Security
{
    internal sealed class SecurityContextConfiguration : DbMigrationsConfiguration<SecurityContext>
    {
        public SecurityContextConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(SecurityContext context)
        {
            
        }
    }
}
