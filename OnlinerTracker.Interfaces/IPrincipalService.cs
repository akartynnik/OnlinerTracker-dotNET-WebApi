using OnlinerTracker.Security;

namespace OnlinerTracker.Interfaces
{
    public interface IPrincipalService
    {
        Principal GetSessionUser();
    }
}
