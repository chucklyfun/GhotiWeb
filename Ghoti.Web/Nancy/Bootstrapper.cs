using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Game;
using GameLogic.User;
using Nancy.Bootstrappers.Ninject;
using Ninject;
using Ninject.Extensions.Factory;
using Utilities.Data;
using Utilities.Data.MongoDb;
using ghoti.web.Controllers;
using Ninject.Extensions.Conventions;
using DataInitializer;
using Newtonsoft.Json;
using MongoDB.Bson;
using Utilities;
using Nancy;
using Nancy.Serialization.JsonNet;
using GameLogic.External;

namespace Ghoti.Web.Nancy
{
    public class Bootstrapper : NinjectNancyBootstrapper
    {
        public static IKernel Kernel { get; set; }

        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            Kernel = kernel;

            kernel.Bind(x => x.FromAssemblyContaining<UserController>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind(x => x.FromAssemblyContaining<EntityBase>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind(x => x.FromAssemblyContaining<LoremIpsum>().SelectAllClasses().BindDefaultInterface());
            kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());

            kernel.Rebind<IGameServices>().To<GameServices>();

            kernel.Rebind<ICrudController<User>>().To<CrudController<User>>();
            kernel.Rebind<ICrudController<Game>>().To<CrudController<Game>>();

            kernel.Rebind<IRepository<User>>().To<MongoDbRepository<User>>().InSingletonScope();
            kernel.Rebind<IRepository<Game>>().To<MongoDbRepository<Game>>().InSingletonScope();
            kernel.Rebind<IRepository<Configuration>>().To<MongoDbRepository<Configuration>>().InSingletonScope();

            kernel.Bind<IConnectionStringProvider>().To<AppConfigConnectionStringProvider>().WithConstructorArgument("connectionStringName", "local");

            kernel.Bind<ISerializer>().To<JsonNetSerializer>();

            kernel.Bind<ISignalRDecisionConnection>().To<SignalRDecisionConnection>();
            kernel.Bind<ISignalRDecisionConnectionFactory>().ToFactory();
        }
    }
}