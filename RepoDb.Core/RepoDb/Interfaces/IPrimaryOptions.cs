using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a property mapping options.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public interface IPrimaryOptions<TEntity> where TEntity : class
    {
        /*
         * Column
         */

        /// <summary>
        /// Maps the equivalent database column of the current primary property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        IPrimaryOptions<TEntity> Column(string column);

        /// <summary>
        /// Maps the equivalent database column of the current primary property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IPrimaryOptions<TEntity> Column(string column,
            bool force);

        /*
         * DbType
         */

        /// <summary>
        /// Maps the equivalent database type of the current primary property.
        /// </summary>
        /// <param name="dbType">The target database type.</param>
        /// <returns>The current instance.</returns>
        IPrimaryOptions<TEntity> DbType(DbType dbType);

        /// <summary>
        /// Maps the equivalent database type of the current primary property.
        /// </summary>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IPrimaryOptions<TEntity> DbType(DbType dbType,
            bool force);
    }
}