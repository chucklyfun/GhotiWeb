using System;
using System.Linq;
using MongoDB.Driver;

namespace Utilities.Data.MongoDb
{
    public class MongoDbContext
    {
        private readonly MongoClient _mongoClient;

        public string DatabaseName { get; set; }

        public MongoDbContext(IConnectionStringProvider connectionStringProvider)
        {
            var connectionString = connectionStringProvider.ConnectionString;
            _mongoClient = new MongoClient(connectionString);
            
            var connectionStringBuilder = new MongoConnectionStringBuilder(connectionString);
            DatabaseName = connectionStringBuilder.DatabaseName;
        }

        public MongoClient MongoClient
        {
            get
            {
                return _mongoClient;
            }
        }

        public MongoServer MongoServer
        {
            get
            {
                return _mongoClient.GetServer();
            }
        }

        public MongoDatabase MongoDatabase
        {
            get
            {
                return _mongoClient.GetServer().GetDatabase(DatabaseName);
            }
        }
    }
}