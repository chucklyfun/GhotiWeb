using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Data
{
    /// <summary>
    /// Generic repository interface
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {

        /// <summary>
        /// Gets entity by id
        /// </summary>
        /// <param name="id">The id.</param>
        TEntity GetById(TKey id);

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        void Delete(TKey id);

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Gets the IQueryable for this repository
        /// </summary>
        IQueryable<TEntity> AsQueryable();

        void RemoveAll();
    }

    /// <summary>
    /// Generic repository interface
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, ObjectId>
        where TEntity : class, IEntity<ObjectId>
    {
    }
}