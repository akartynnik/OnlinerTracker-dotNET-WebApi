using System.ComponentModel.Composition;
using System.Data.Entity;

namespace OnlinerTracker.Data.Context
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class TrackerContext : DbContext
    {
        public TrackerContext() : base("DataConnection")
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
