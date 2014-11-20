using System;
using System.Linq;
using GameLogic.Game;
using GameLogic.User;
using NUnit.Framework;
using Ninject;
using Ninject.Extensions.Conventions;
using Utilities.Data;
using Utilities.Data.MongoDb;

namespace DataInitializer.Test
{
    
    public class InitializeData
    {
        public IKernel IntegrationsKernel {get;set;}
        
        [SetUp]
        public void Setup()
        {
            IntegrationsKernel = new StandardKernel();

            IntegrationsKernel.Bind(x => x.FromAssemblyContaining<LoremIpsum>().SelectAllClasses().BindDefaultInterface());
            IntegrationsKernel.Bind(x => x.FromAssemblyContaining<EntityBase>().SelectAllClasses().BindDefaultInterface());
            IntegrationsKernel.Bind(x => x.FromThisAssembly().SelectAllClasses().BindDefaultInterface());

            IntegrationsKernel.Rebind<IRepository<User>>().To<MongoDbRepository<User>>().InSingletonScope();
            IntegrationsKernel.Rebind<IRepository<Game>>().To<MongoDbRepository<Game>>().InSingletonScope();

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
