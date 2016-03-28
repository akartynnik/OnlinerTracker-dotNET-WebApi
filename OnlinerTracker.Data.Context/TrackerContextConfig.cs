using System.Data.Entity.Migrations;

namespace OnlinerTracker.Data.Context
{
    internal sealed class Configuration : DbMigrationsConfiguration<TrackerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
