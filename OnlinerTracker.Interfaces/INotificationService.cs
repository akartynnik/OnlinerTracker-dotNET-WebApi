using System.Threading.Tasks;

namespace OnlinerTracker.Interfaces
{
    public interface INotificationService
    {
        Task<string> CheckNotifications();
    }
}
