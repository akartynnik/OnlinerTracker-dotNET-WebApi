using System.ComponentModel.Composition;
using System.Data.Entity;

namespace OnlinerTracker.Data.Context
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class OnlinerTrackerContext : DbContext
    {
        public OnlinerTrackerContext() : base("DataConnection")
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}
