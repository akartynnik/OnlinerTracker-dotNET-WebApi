using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using System;
using System.Linq;

namespace OnlinerTracker.Services
{
    public class LogService : ILogService
    {
        private readonly TrackerContext _context;

        public LogService()
        {
            _context = new TrackerContext();
        }

        public void AddJobLog(JobType logType, string info, bool isSuccessed = true)
        {
            _context.JobLogs.Add(new JobLog
            {
                Type = logType,
                CheckedAt = DateTime.Now,
                IsSuccessed = isSuccessed,
                Info = info
            });
            _context.SaveChanges();
        }

        public JobLog GetLastSuccessLog(JobType logType)
        {
            return _context.JobLogs.OrderByDescending(u => u.CheckedAt).FirstOrDefault(u => u.Type == logType && u.IsSuccessed);
        }
    }
}
