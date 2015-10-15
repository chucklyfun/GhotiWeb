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
using GameLogic.Player;
using GameLogic.Deck;
using System.Reflection;
using MongoDB.Bson.Serialization;


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
            kernel.Rebind<IDecisionMakerManager>().To<DecisionMakerManager>().InSingletonScope();
            kernel.Rebind<IGameManager>().To<GameManager>().InSingletonScope();
            kernel.Rebind<IPlayerManager>().To<PlayerManager>().InSingletonScope();
            kernel.Rebind<IGameViewManager>().To<GameViewManager>().InSingletonScope();
            kernel.Rebind<IGameStateManager>().To<GameStateManager>().InSingletonScope();
            kernel.Rebind<ICardManager<IMonsterCard>>().To<CardManager<IMonsterCard>>().InSingletonScope();
            kernel.Rebind<ICardManager<IPlayerCard>>().To<CardManager<IPlayerCard>>().InSingletonScope();
            kernel.Rebind<ICardUtilities<IPlayerCard>>().To<CardUtilities<IPlayerCard>>().InSingletonScope();
            kernel.Rebind<ICardUtilities<IMonsterCard>>().To<CardUtilities<IMonsterCard>>().InSingletonScope();
            kernel.Rebind<IGameUtilities>().To<GameUtilities>().InSingletonScope();

            kernel.Bind<IConnectionStringProvider>().To<AppConfigConnectionStringProvider>().WithConstructorArgument("connectionStringName", "local");

            kernel.Bind<ISerializer>().To<JsonNetSerializer>();

            kernel.Bind<ISignalRDecisionConnection>().To<SignalRDecisionConnection>();
            kernel.Bind<ISignalRDecisionConnectionFactory>().ToFactory();


            var types = Assembly.GetAssembly(typeof(Game))
                    .GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(EntityBase)));
            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

        }
    }
}