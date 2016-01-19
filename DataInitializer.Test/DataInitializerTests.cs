using System;
using System.Linq;
using GameLogic.Game;
using NUnit.Framework;
using Ninject;
using Utilities.Data;
using Utilities.Data.MongoDb;
using GameLogic.Domain;

namespace DataInitializer.Test
{
    
    public class InitializeData
    {
        public IKernel IntegrationsKernel {get;set;}
        
        [SetUp]
        public void Setup()
        {
            IntegrationsKernel = new StandardKernel();

            IntegrationsKernel.Bind<IRepository<User>>().To<MongoDbRepository<User>>().InSingletonScope();
            IntegrationsKernel.Bind<IRepository<Game>>().To<MongoDbRepository<Game>>().InSingletonScope();

            IntegrationsKernel.Bind<IConnectionStringProvider>().To<AppConfigConnectionStringProvider>().WithConstructorArgument("connectionStringName", "local");
        }

        [Test]
        public void InitializeUsers()
        {
            var dataInitializer = IntegrationsKernel.Get<DataInitializer.Initializer>();
            dataInitializer.InitializeAdminUser();
            dataInitializer.InitializePlayers(10);
        }
    }
}
