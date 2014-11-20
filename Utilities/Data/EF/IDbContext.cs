using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Utilities.Data.Ef
{
    /// <summary>
    /// Interface for DbContext UnitOfWork pattern
    /// </summary>
    public interface IDbContext: IUnitOfWork
    {
        /// <summary>
        /// Returns a DbSet instance for access to entities of the given type in the context, the ObjectStateManager, and the underlying store.
        /// </summary>
        /// <typeparam name="TEntity">A set for the given entity type.</typeparam>
        /// <returns>TEntity: The type entity for which a set should be returned.</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>Creates a raw SQL query that will return elements of the given type. The type can be any type that has properties that match the 
        /// names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. 
        /// The results of this query are never tracked by the context even if the type of object returned is an entity type. 
        /// Use the SqlQuery method to return entities that are tracked by the context. </summary>
        /// <typeparam name="TEntity">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>A IEnumerable object that will execute the query when it is enumerated.</returns>
        IEnumerable<TEntity> SqlQuery<TEntity>(string sql, params object[] parameters) where TEntity : class, new();

        /// <summary>Attaches the entity to the context.  It is placed in the Unchanged state as if it was just read from the database</summary>
        /// <typeparam name="TEntity">The type of object to be attached</typeparam>
        /// <param name="entity">The entity to attach</param>
        /// <param name="markDirty">Marks the newly attached entity as modified</param>
        /// <returns></returns>
        TEntity Attach<TEntity>(TEntity entity, bool markDirty = false) where TEntity : class;

        /// <summary>
        /// Returns true if database exists
        /// </summary>
        /// <returns></returns>
        bool DatabaseExists();
    }
}