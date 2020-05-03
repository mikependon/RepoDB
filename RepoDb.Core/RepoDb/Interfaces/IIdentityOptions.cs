namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be an identity mapping options.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public interface IIdentityOptions<TEntity>
        where TEntity : class
    {
        /*
         * Column
         */

        /// <summary>
        /// Maps the equivalent database column of the current identity property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        IIdentityOptions<TEntity> Column(string column);

        /// <summary>
        /// Maps the equivalent database column of the current identity property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IIdentityOptions<TEntity> Column(string column,
            bool force);
    }
}