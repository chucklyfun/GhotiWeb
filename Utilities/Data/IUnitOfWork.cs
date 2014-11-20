using System;
using System.Linq;

namespace Utilities.Data
{
    /// <summary>
    /// Interface for the UnitOfWork pattern
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all changes made in this context to the underlying database
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        int SaveChanges();
    }
    
}