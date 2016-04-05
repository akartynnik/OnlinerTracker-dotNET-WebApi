using FluentScheduler;
using OnlinerTracker.Interfaces;

namespace OnlinerTracker.Api.Jobs
{
    public class TrackingJob : IJob
    {
        private readonly ITrackingService _trackingService;
        public TrackingJob(ITrackingService trackingService)
        {
            _trackingService = trackingService;
        }
        private readonly object _lock = new object();

        public void Execute()
        {
            lock (_lock)
                _trackingService.CheckProducts();
        }
    }
}