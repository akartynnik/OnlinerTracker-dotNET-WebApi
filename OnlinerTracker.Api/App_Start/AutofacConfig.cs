using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using OnlinerTracker.Api.Hubs;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Proxies;
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
            
            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.Register(c => new DialogService(GlobalHost.ConnectionManager.GetHubContext<DialogHub>()))
                .As<IDialogService>()
                .InstancePerLifetimeScope();
            builder.Register(c => new OnlinerProxy(c.Resolve<IProductService>()))
                .As<IExternalProductService>()
                .InstancePerLifetimeScope();
            builder.Register(c => new TrackingService(c.Resolve<IProductService>(), c.Resolve<IExternalProductService>())
                    {
                        MinutesBeforeCheck = ConfigurationManager.AppSettings["schedulerConfig:minutesBeforeCheckCost"]
                    })
                .As<ITrackingService>()
                .InstancePerLifetimeScope();

            #endregion

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            iocContainer = container;
        }
    }
}