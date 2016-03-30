using System.Data.Entity;

namespace OnlinerTracker.Data.Context
{
    public class TrackerContext : DbContext
    {
        public TrackerContext() : base("DataConnection")
        {
            Database.SetInitializer(new TrackerInitializer());
        }
        public DbSet<Product> Products { get; set; }
    }
}
