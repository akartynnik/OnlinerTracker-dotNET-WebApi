using FluentScheduler;

namespace OnlinerTracker.Interfaces
{
    public interface ITrackingService : IJob
    {
        void StartCheck();
    }
}