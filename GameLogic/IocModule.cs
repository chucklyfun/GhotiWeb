using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Domain;
using GameLogic.Game;
using Ninject;
using Ninject.Extensions.Factory;
using Utilities.Data;
using Utilities.Data.MongoDb;
using Newtonsoft.Json;
using MongoDB.Bson;
using Utilities;
using GameLogic.External;
using GameLogic.Player;
using GameLogic.Deck;
using System.Reflection;
using MongoDB.Bson.Serialization;
using GameLogic.Data;


namespace GameLogic
{
    public class IocModule : IIocModule
    {
        public bool Bind(IKernel kernel)
        {
            kernel.Bind<IGameServices>().To<GameServices>().InSingletonScope();

            kernel.Bind<IRepository<User>>().To<MongoDbRepository<User>>().InSingletonScope();
            kernel.Bind<IRepository<Domain.Game>>().To<MongoDbRepository<Domain.Game>>().InSingletonScope();
            kernel.Bind<IRepository<Domain.DecisionMaker>>().To<MongoDbRepository<Domain.DecisionMaker>>().InSingletonScope();

            kernel.Bind<IDecisionMakerManager>().To<DecisionMakerManager>().InSingletonScope();
            kernel.Bind<IGameManager>().To<GameManager>().InSingletonScope();
            kernel.Bind<IGameViewManager>().To<GameViewManager>().InSingletonScope();
            kernel.Bind<IGameStateManager>().To<GameStateManager>();
            kernel.Bind<IPlayerManager>().To<PlayerManager>().InSingletonScope();
            kernel.Bind<IGameStateManager>().To<GameStateManager>().InSingletonScope();
            kernel.Bind<ICardManager>().To<CardManager>().InSingletonScope();
            kernel.Bind<ICardUtilities>().To<CardUtilities>().InSingletonScope();
            kernel.Bind<IGameUtilities>().To<GameUtilities>().InSingletonScope();
            kernel.Bind<ICardLoader>().To<CardLoader>().InSingletonScope();
            kernel.Bind<ICardService>().To<CardService>().InSingletonScope();

            kernel.Bind<ISignalRDecisionConnection>().To<SignalRDecisionConnection>();
            kernel.Bind<ISignalRDecisionConnectionFactory>().ToFactory();

            var types = Assembly.GetAssembly(typeof(Domain.Game))
                    .GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(EntityBase)));
            foreach (var t in types)
                BsonClassMap.LookupClassMap(t);

            return true;
        }
    }
}