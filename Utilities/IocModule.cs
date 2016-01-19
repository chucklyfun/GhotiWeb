using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Data;
using Utilities.Data.MongoDb;
using Utilities;
using System.Reflection;
using MongoDB.Bson.Serialization;
using Ninject;

namespace Utilities
{
    public class IocModule : IIocModule
    {
        public bool Bind(IKernel kernel)
        {

            kernel.Bind<IConnectionStringProvider>().To<AppConfigConnectionStringProvider>().WithConstructorArgument("connectionStringName", "local");
            kernel.Bind<ICsvReader>().To<CsvReader>();
            kernel.Bind<ISerializationService>().To<SerializationService>();
            kernel.Bind<ISettingsManager>().To<SettingsManager>();
            kernel.Bind<IRepository<Configuration>>().To<MongoDbRepository<Configuration>>();

            return true;
        }
    }
}