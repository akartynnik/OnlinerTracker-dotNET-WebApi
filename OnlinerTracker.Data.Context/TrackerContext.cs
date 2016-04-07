using System.Data.Entity;

namespace OnlinerTracker.Data.Context
{
    public class TrackerContext : DbContext
    {
        public TrackerContext() : base("DataConnection")
        {
            Database.SetInitializer(new TrackerInitializer());
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Cost> Costs { get; set; }

        public DbSet<JobLog> JobLogs { get; set; }
    }
}
