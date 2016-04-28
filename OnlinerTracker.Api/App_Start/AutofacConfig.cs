using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using AutoMapper.Mappers;
using Microsoft.AspNet.SignalR;
using OnlinerTracker.Api.Jobs;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Proxies;
using OnlinerTracker.Security;
using OnlinerTracker.Services;
using OnlinerTracker.Services.BaseServices;
using OnlinerTracker.Services.Configs;
using OnlinerTracker.Services.Contexts;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Api.Models.Configs;
using MessageSenderConfig = OnlinerTracker.Data.MessageSenderConfig;

namespace OnlinerTracker.Api
{
    public class AutofacConfig
    {
        public static void Register(HttpConfiguration config, out IContainer iocContainer)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<TrackingJob>().As<TrackingJob>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationJob>().As<NotificationJob>().InstancePerLifetimeScope();

            builder.RegisterType<TrackerContext>().As<TrackerContext>().InstancePerLifetimeScope();
            builder.RegisterType<SecurityRepository>().As<ISecurityRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterType<LogService>().As<ILogService>().InstancePerLifetimeScope();
            builder.RegisterType<NbrbProxy>().As<ICurrencyService>().InstancePerLifetimeScope();
            builder.RegisterType<StringComposer>().As<IStringComposer>().InstancePerLifetimeScope();

            builder.Register(c => new OnlinerProxy(c.Resolve<IProductService>()))
                .As<IExternalProductService>()
                .InstancePerLifetimeScope();
            builder.Register(c => new PrincipalService(HttpContext.Current))
                .As<IPrincipalService>()
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

            #region IMessager register

            builder.Register(
                c =>
                    new MessageSender(
                        new MessageSenderConfig
                        {
                            SmtpAccount = ConfigurationManager.AppSettings["notifConfig:smtpAccount"],
                            EmailSenderName = ConfigurationManager.AppSettings["notifConfig:emailSenderName"],
                            SmtpHost = ConfigurationManager.AppSettings["notifConfig:smtpServer"],
                            SmtpPortString = ConfigurationManager.AppSettings["notifConfig:smtpPort"],
                            SmtpPassword = ConfigurationManager.AppSettings["notifConfig:smtpPassword"]
                        }
                        ))
                .As<IMessageSender>()
                .InstancePerLifetimeScope();

            #endregion

            #region Notification service register

            builder.Register(
                c =>
                    new NotificationService( 
                        new NotificationServiceConfig()
                        {
                            ProductService = c.Resolve<IProductService>(),
                            LogService = c.Resolve<ILogService>(),
                            SecurityRepository = c.Resolve<ISecurityRepository>(),
                            StringComposer = c.Resolve<IStringComposer>(),
                            MessageSender = c.Resolve<IMessageSender>()
                        }
                        ))
                .As<INotificationService>()
                .InstancePerLifetimeScope();

            #endregion

            #region Dialog service register

            switch (ConfigurationManager.AppSettings["dialogServiceProvider"])
            {
                case "zeromq":
                    builder.RegisterInstance(
                        new NetMqDialogContext(ConfigurationManager.AppSettings["zeroMq:routerUrl"],
                            ConfigurationManager.AppSettings["zeroMq:publisherUrl"])).SingleInstance();
                    builder.Register(c => new NetMqService(c.Resolve<NetMqDialogContext>()))
                        .As<IDialogService>()
                        .InstancePerLifetimeScope();
                    break;
                default: //signalr
                    builder.Register(
                        c => new SignalRService(GlobalHost.ConnectionManager.GetHubContext<SignalRDialogHub>()))
                        .As<IDialogService>()
                        .InstancePerLifetimeScope();
                    break;
            }

            #endregion

            #region Automapper register

            var profiles =
                from t in typeof (AutomapperMappingProfile).Assembly.GetTypes()
                where typeof (Profile).IsAssignableFrom(t)
                select (Profile) Activator.CreateInstance(t);

            builder.Register(ctx => new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            }));

            builder.Register(ctx => ctx.Resolve<MapperConfiguration>().CreateMapper()).As<IMapper>();

            #endregion

            builder.Register(
                c =>
                    new ProductsController(
                        new ProductsControllerConfig
                        {
                            ProductService = c.Resolve<IProductService>(),
                            DialogService = c.Resolve<IDialogService>(),
                            ExternalProductService = c.Resolve<IExternalProductService>(),
                            Mapper = c.Resolve<IMapper>()
                        }, c.Resolve<IPrincipalService>()
                        ))
                .As<ProductsController>()
                .InstancePerLifetimeScope();

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            iocContainer = container;
        }
    }
}