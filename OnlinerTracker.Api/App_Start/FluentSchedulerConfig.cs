using Autofac;
using Autofac.Integration.WebApi;
using FluentScheduler;
using OnlinerTracker.Api.Jobs;
using System.Web.Http;

namespace OnlinerTracker.Api
{
    public class FluentSchedulerConfig : Registry
    {
        public FluentSchedulerConfig()
        {
            Schedule<TrackingJob>().ToRunNow().AndEvery(5).Minutes();
            Schedule<NotificationJob>().ToRunNow().AndEvery(6).Minutes();
        }
    }

    public class JobFactory : IJobFactory
    {
        private HttpConfiguration Config { get; set; }
        public JobFactory(HttpConfiguration config)
        {
            Config = config;
        }
        public IJob GetJobInstance<T>() where T : IJob
        {
            var scope = Config.DependencyResolver.GetRootLifetimeScope();
            return scope.Resolve<T>();
        }
    }
}