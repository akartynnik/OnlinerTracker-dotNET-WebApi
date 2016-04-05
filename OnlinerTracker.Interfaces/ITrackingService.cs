using System.Threading.Tasks;

namespace OnlinerTracker.Interfaces
{
    public interface ITrackingService
    {
        Task<string> CheckProducts();
    }
}