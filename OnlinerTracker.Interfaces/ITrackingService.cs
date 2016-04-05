using FluentScheduler;
using System.Threading.Tasks;

namespace OnlinerTracker.Interfaces
{
    public interface ITrackingService : IJob
    {
        Task<string> CheckProduct();
    }
}