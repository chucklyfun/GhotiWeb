using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Utilities.Data.Ef
{
    /// <summary>
    /// Represents the DataContext
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DataContext : DbContext, IDbContext
    {
        public DataContext()
            : base ("VendorCatalogs")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public bool DatabaseExists()
        {
            return Database.Exists();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public IEnumerable<TEntity> SqlQuery<TEntity>(string sql, params object[] parameters) where TEntity : class, new()
        {
            return Database.SqlQuery<TEntity>(sql, parameters);
        }

        public TEntity Attach<TEntity>(TEntity entity, bool markDirty = false) where TEntity : class
        {
            //if (Entry(entity).State == System.Data.EntityState.Detached)
            //{
            //    this.Set<TEntity>().Attach(entity);

            //    if (markDirty)
            //    {
            //        Entry(entity).State = System.Data.EntityState.Modified;
            //    }
            //}

            return entity;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var typesToRegister = GetConfigurations();

            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            // Remove undesired conventions
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        protected IEnumerable<Type> GetConfigurations()
        {
            var configType = GetType();
            var typesToRegister =
                Assembly.GetAssembly(configType)
                        .GetTypes()
                        .Where(type => !string.IsNullOrEmpty(type.Namespace))
                        .Where(
                            type =>
                            type.BaseType != null && 
                            type.BaseType.IsGenericType && 
                            type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)
                            );

            return typesToRegister;
        }
    }
}