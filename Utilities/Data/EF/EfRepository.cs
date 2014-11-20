using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Utilities.Data.Ef
{
    /// <summary>
    /// Entity Framework repository
    /// </summary>
    public partial class EfRepository<T, TKey> : IRepository<T, TKey>
        where T : class, IEntity<TKey>
    {
        private readonly IDbContext _context;

        private IDbSet<T> _entities;

        /// <summary>Constructor</summary>
        /// <param name="context">Object context</param>
        public EfRepository(IDbContext context)
        {
            _context = context;
        }

        private IDbSet<T> Entities
        {
            get
            {
                return _entities ?? (_entities = _context.Set<T>());
            }
        }

        public T GetById(TKey id)
        {
            return Entities.Find(id);
        }

        /// <summary>Attaches the entity to the underlying context of this reposity.  m  
        /// It is placed in the Unchanged state as if it was just read from the database</summary>
        /// <param name="entity">The entity to attach</param>
        /// <param name="markDirty">Marks the newly attached entity as modified</param>
        public void Attach(T entity, bool markDirty = false)
        {
            _context.Attach(entity, markDirty);
        }

        public T Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                Entities.Add(entity);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = GetMessageFromDbEntityValidationException(dbEx);
                var fail = new Exception(msg, dbEx);
                throw fail;
            }

            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public void Insert(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            try
            {
                foreach (var entity in entities)
                {
                    Entities.Add(entity); 
                }
                
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = GetMessageFromDbEntityValidationException(dbEx);
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public T Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                _context.Attach(entity);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var msg = GetMessageFromDbEntityValidationException(ex);
                var fail = new Exception(msg, ex);
                throw fail;
            }

            return entity;
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public void Update(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            try
            {
                foreach (var entity in entities)
                {
                    Entities.AddOrUpdate(entity);
                }

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = GetMessageFromDbEntityValidationException(dbEx);
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public void Delete(TKey id)
        {
            var entity = GetById(id);
            Delete(entity);
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                Entities.Remove(entity);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var msg = GetMessageFromDbEntityValidationException(ex);
                var fail = new Exception(msg, ex);
                throw fail;
            }
        }

        /// <summary>
        /// Gets the IQueryable for this repository
        /// </summary>
        public IQueryable<T> AsQueryable()
        {
            return Entities;
        }
        
        /// <summary>Selects query with included properties</summary>
        /// <param name="includeProperties"></param>
        public IQueryable<T> SelectWith(params Expression<Func<T, object>>[] includeProperties)
        {
            return includeProperties.Aggregate(AsQueryable(), (current, includeProperty) => current.Include(includeProperty));
        }

        public IQueryable<T> SelectWith(params string[] includeProperties)
        {
            return includeProperties.Aggregate(AsQueryable(), (current, includeProperty) => current.Include(includeProperty));
        }

        [ExcludeFromCodeCoverage]
        private string GetMessageFromDbEntityValidationException(DbEntityValidationException ex)
        {
            var sb = new StringBuilder();
            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    sb.AppendFormat("Property: {0} Error: {1}" + Environment.NewLine, validationError.PropertyName, validationError.ErrorMessage);
                }
            }

            return sb.ToString();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }
    }

}