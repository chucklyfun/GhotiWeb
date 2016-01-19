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


namespace DataInitializer
{
    public class IocModule : IIocModule
    {
        public bool Bind(IKernel kernel)
        {
            kernel.Bind<ILoremIpsum>().To<LoremIpsum>();
            kernel.Bind<IInitializer>().To<Initializer>();

            return true;
        }
    }
}