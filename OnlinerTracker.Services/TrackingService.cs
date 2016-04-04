using OnlinerTracker.Interfaces;
using System.Diagnostics;

namespace OnlinerTracker.Services
{
    public class TrackingService : ITrackingService
    {
        private readonly object _lock = new object();
        private bool _shuttingDown;

        public void StartCheck()
        {
            Debug.WriteLine("ololo");
            //_trackingService.CheckCost(Guid.Empty);
        }

        public void Execute()
        {
            lock (_lock)
            {
                if (_shuttingDown)
                    return;
                StartCheck();
            }
        }
    }
}