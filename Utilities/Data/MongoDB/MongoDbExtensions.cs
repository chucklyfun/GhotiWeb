using System;
using System.Linq;
using MongoDB.Driver;

namespace Utilities.Data.MongoDb
{
    public static class MongoDbExtensions
    {
        public static MongoCollection<TEntity> GetCollection<TEntity>(this MongoDatabase mongoDatabase)
        {
            return mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name);
        }
    }
}