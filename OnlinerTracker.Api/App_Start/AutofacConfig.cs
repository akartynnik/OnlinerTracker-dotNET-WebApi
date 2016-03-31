using Autofac;
using Autofac.Integration.WebApi;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Services;
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

            builder.RegisterType<AuthorizationService>().As<IAuthorizationService>();
            builder.RegisterType<ProductService>().As<IProductService>();
            //builder.RegisterType<CheckStatusJob>().As<ICheckStatusJob>();
            builder.Register(c => new TrackingService()).As<ITrackingService>();

            #endregion

            var container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;
            iocContainer = container;
        }
    }
}