using System.Data.Entity;

namespace OnlinerTracker.Data.Context
{
    public class TrackerInitializer : DropCreateDatabaseIfModelChanges<TrackerContext>
    {
        protected override void Seed(TrackerContext context)
        {
            
        }
    }
}
