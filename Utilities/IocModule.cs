using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Data;
using Utilities.Data.MongoDb;
using Utilities;
using System.Reflection;
using MongoDB.Bson.Serialization;
using Ninject;
using Utilities.Data.Cache;
using Utilities.EventBroker;
using System.Threading;

namespace Utilities
{
    public class IocModule : IIocModule
    {
        public bool Bind(IKernel kernel)
        {
            kernel.Bind<ISignalRConnectionManager>().To<SignalRConnectionManager>().InSingletonScope();
            kernel.Bind<IConnectionStringProvider>().To<AppConfigConnectionStringProvider>().WithConstructorArgument("connectionStringName", "local");
            kernel.Bind<ICsvReader>().To<CsvReader>().InSingletonScope();
            kernel.Bind<ISerializationService>().To<SerializationService>().InSingletonScope();
            kernel.Bind<ISettingsManager>().To<SettingsManager>().InSingletonScope();
            kernel.Bind<IRepository<Configuration>>().To<MongoDbRepository<Configuration>>().InSingletonScope();
            kernel.Bind<IRuntimeCache>().To<RuntimeCache>().InSingletonScope();
            kernel.Bind<IEventPublisher>().To<EventPublisher>().InSingletonScope();
            kernel.Bind<ISubscriptionService>().To<SubscriptionService>().InSingletonScope();
            kernel.Bind<ICollectionService>().To<CollectionService>();
            kernel.Bind<ReaderWriterLockSlim>().To<ReaderWriterLockSlim>();
            

            return true;
        }
    }
}