using Autofac;
using Autofac.Integration.WebApi;
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

            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().InstancePerDependency();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerDependency();
            builder.Register(c => new TrackingService()).As<ITrackingService>().InstancePerDependency();
            builder.Register(c => new OnlinerProxy(c.Resolve<IProductService>())).As<IExternalProductService>().InstancePerDependency();

            #endregion

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            iocContainer = container;
        }
    }
}