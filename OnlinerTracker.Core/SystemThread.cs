using System.Threading;

namespace OnlinerTracker.Core
{
    public class SystemThread
    {
        private static int _customSleepTimeInMillisec;

        public static void SetSleepTime(int customSleepTimeInMillisec)
        {
            _customSleepTimeInMillisec = customSleepTimeInMillisec;
        }

        public static void ResetSleepTime()
        {
            _customSleepTimeInMillisec = -1;
        }

        public static void Sleep(int sleepTimeInMillisec)
        {
            if (_customSleepTimeInMillisec != -1)
            {
                Thread.Sleep(_customSleepTimeInMillisec);
                return;
            }
            Thread.Sleep(sleepTimeInMillisec);
        }
    }
}
