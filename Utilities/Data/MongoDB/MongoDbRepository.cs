using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Utilities.Data.MongoDb
{
    public class MongoDbRepository<T, TKey> : IRepository<T, TKey>
        where T : class, IEntity<TKey>
    {
        private readonly MongoCollection<T> _collection;

        public MongoDbRepository(MongoDbContext mongoDbContext)
        {
            _collection = mongoDbContext.MongoDatabase.GetCollection<T>();

        }

        /// <summary>
        /// Gets entity by id
        /// </summary>
        /// <param name="id">The id.</param>
        public T GetById(TKey id)
        {
            if ( typeof(T).IsSubclassOf(typeof(IEntity<T>)))
            {
                return _collection.FindOneByIdAs<T>(new ObjectId(id as string));
            }

            return _collection.FindOneByIdAs<T>(BsonValue.Create(id));
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public T Insert(T entity)
        {
            _collection.Insert<T>(entity);
            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public void Insert(IEnumerable<T> entities)
        {
            _collection.InsertBatch<T>(entities);
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public T Update(T entity)
        {
            _collection.Save<T>(entity);

            return entity;
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                _collection.Save<T>(entity);
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public virtual void Delete(TKey id)
        {
            if (typeof(T).IsSubclassOf(typeof(IEntity)))
            {
                _collection.Remove(Query.EQ("_id", new ObjectId(id as string)));
            }
            else
            {
                _collection.Remove(Query.EQ("_id", BsonValue.Create(id)));
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its ObjectId.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        public virtual void Delete(string id)
        {
            _collection.Remove(Query.EQ("_id", id));
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(T entity)
        {
            Delete(entity.Id);
        }

        /// <summary>
        /// Gets the IQueryable for this repository
        /// </summary>
        public IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public void RemoveAll()
        {
            _collection.RemoveAll();
        }
    }

    public class MongoDbRepository<T> : MongoDbRepository<T, ObjectId>, IRepository<T>
        where T : class, IEntity<ObjectId>
    {
        public MongoDbRepository(MongoDbContext mongoDbContext)
            : base(mongoDbContext)
        {
        }
    }
}
