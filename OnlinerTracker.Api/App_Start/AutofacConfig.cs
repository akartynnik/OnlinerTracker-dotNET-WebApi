using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using OnlinerTracker.Api.Hubs;
using OnlinerTracker.Api.Jobs;
using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Proxies;
using OnlinerTracker.Security;
using OnlinerTracker.Services;
using System.Configuration;
using System.Reflection;
using System.Web.Http;

namespace OnlinerTracker.Api
{
    public class AutofacConfig
    {
        public static void Register(HttpConfiguration config, out IContainer iocContainer)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            #region Dependensies

            builder.RegisterType<SecurityRepository>().As<SecurityRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TrackerContext>().As<TrackerContext>().InstancePerLifetimeScope();

            builder.RegisterType<TrackingJob>().As<TrackingJob>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationJob>().As<NotificationJob>().InstancePerLifetimeScope();

            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerLifetimeScope();

            builder.Register(c => new DialogService(GlobalHost.ConnectionManager.GetHubContext<DialogHub>()))
                .As<IDialogService>()
                .InstancePerLifetimeScope();
            builder.Register(c => new OnlinerProxy(c.Resolve<IProductService>()))
                .As<IExternalProductService>()
                .InstancePerLifetimeScope();
            builder.Register(
                c =>
                    new TrackingService(c.Resolve<IProductService>(), c.Resolve<IExternalProductService>(),
                        c.Resolve<ILogService>())
                    {
                        MinutesBeforeCheck = ConfigurationManager.AppSettings["schedulerConfig:minutesBeforeCheckCost"]
                    })
                .As<ITrackingService>()
                .InstancePerLifetimeScope();
            builder.Register(
                c =>
                    new NotificationService(c.Resolve<IProductService>(), c.Resolve<ILogService>(),
                        c.Resolve<SecurityRepository>(),
                        new NotificationConfig()
                        {
                            SmtpHost = ConfigurationManager.AppSettings["notifConfig:smtpServer"],
                            SmtpPortString = ConfigurationManager.AppSettings["notifConfig:smtpPort"],
                            SmtpAccount = ConfigurationManager.AppSettings["notifConfig:smtpAccount"],
                            SmtpPassword = ConfigurationManager.AppSettings["notifConfig:smtpPassword"]
                        }
                        )
                    {
                        EmailSenderName = ConfigurationManager.AppSettings["notifConfig:emailSenderName"],
                        HourInWhichSendingStart =
                            ConfigurationManager.AppSettings["schedulerConfig:hourInWhichSendingStart"]
                    })
                .As<INotificationService>()
                .InstancePerLifetimeScope();

            #endregion

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            iocContainer = container;
        }
    }
}