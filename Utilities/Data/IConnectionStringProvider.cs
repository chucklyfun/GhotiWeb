using System;
using System.Configuration;
using System.Linq;

namespace Utilities.Data
{
    public interface IConnectionStringProvider
    {
        string ConnectionString { get; }
    }

    public class AppConfigConnectionStringProvider : IConnectionStringProvider
    {
        public AppConfigConnectionStringProvider(string connectionStringName)
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
        
        public string ConnectionString { get; private set; }
    }

    
}