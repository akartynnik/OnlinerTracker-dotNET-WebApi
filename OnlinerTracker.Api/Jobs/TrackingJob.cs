using FluentScheduler;
using OnlinerTracker.Interfaces;
using System.Configuration;

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
            {
                var minutesBeforeCheckCost = int.Parse(ConfigurationManager.AppSettings["schedulerConfig:minutesBeforeCheckCost"]);
                _trackingService.CheckProducts(minutesBeforeCheckCost);
            }
        }
    }
}