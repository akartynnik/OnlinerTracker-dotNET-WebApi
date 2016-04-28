using Microsoft.AspNet.Identity.EntityFramework;

namespace OnlinerTracker.Security
{
    public class ApplicationUser: IdentityUser, IApplicationUser
    { 
        public int NotificationHour { get; set; }
    }
}
