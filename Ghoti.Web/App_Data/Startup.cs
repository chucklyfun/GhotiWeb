using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Nancy;
using Nancy.Serialization.JsonNet;
using Nancy.Bootstrappers.Ninject;
using Ninject;
using Ninject.Extensions.Factory;
using Utilities;

[assembly: OwinStartup(typeof(Ghoti.Web.Startup))]
namespace Ghoti.Web
{

    public class Startup : NinjectNancyBootstrapper
    {
        public static IKernel Kernel { get; set; }

        public static NinjectSignalRDependencyResolver _signalRDependencyResolver { get; set; }

        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;

            _signalRDependencyResolver = new NinjectSignalRDependencyResolver();
            hubConfiguration.Resolver = _signalRDependencyResolver;

            app.MapSignalR(hubConfiguration);
        }

        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            Kernel = kernel;

            IIocModule module = new Utilities.IocModule();
            module.Bind(Kernel);

            module = new DataInitializer.IocModule();
            module.Bind(Kernel);

            module = new GameLogic.IocModule();
            module.Bind(Kernel);

            module = new Ghoti.Web.IocModule();
            module.Bind(Kernel);

            _signalRDependencyResolver.Kernel = kernel;
        }
    }


    public class NinjectSignalRDependencyResolver : DefaultDependencyResolver
    {
        public IKernel Kernel { get; set; }

        public override object GetService(Type serviceType)
        {
            return Kernel == null ? base.GetService(serviceType) : Kernel.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel == null ? base.GetServices(serviceType) : Kernel.GetAll(serviceType).Concat(base.GetServices(serviceType)) ?? base.GetServices(serviceType);
        }
    }
}