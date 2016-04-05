using OnlinerTracker.Data;

namespace OnlinerTracker.Interfaces
{
    public interface ILogService
    {
        void AddJobLog(JobType logType, string info, bool isSuccessed = true);

        JobLog GetLastSuccessLog(JobType logType);
    }
}
