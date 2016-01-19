using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Game;
using Nancy.Bootstrappers.Ninject;
using Ninject;
using Ninject.Extensions.Factory;
using Utilities.Data;
using Utilities.Data.MongoDb;
using ghoti.web.Controllers;
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

            IIocModule module = new Utilities.IocModule();
            module.Bind(kernel);

            module = new DataInitializer.IocModule();
            module.Bind(kernel);

            module = new GameLogic.IocModule();
            module.Bind(kernel);

            module = new Ghoti.Web.IocModule();
            module.Bind(kernel);
        }
    }
}