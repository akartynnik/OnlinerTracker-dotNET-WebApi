using System.Threading.Tasks;

namespace OnlinerTracker.Interfaces
{
    public interface ITrackingService
    {
        Task CheckProducts(int minutesBeforeCheck);

        Task CheckProducts();
    }
}