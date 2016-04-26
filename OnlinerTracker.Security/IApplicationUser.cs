using Microsoft.AspNet.Identity;

namespace OnlinerTracker.Security
{
    public interface IApplicationUser: IUser
    {
        string Email { get; set; }
    }
}
