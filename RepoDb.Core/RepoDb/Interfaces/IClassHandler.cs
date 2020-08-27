using System.Data.Common;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface that is used to mark a class to be a class handler.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public interface IClassHandler<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// The method that is being invoked when the outbound transformation is triggered (ie: Query).
        /// </summary>
        /// <param name="entity">The current instance of data entity object.</param>
        /// <param name="reader">The instance of <see cref="DbDataReader"/> in used to populate the data entity object.</param>
        /// <returns>The current or the newly created instance data entity object.</returns>
        TEntity Get(TEntity entity,
            DbDataReader reader);

        /// <summary>
        /// The method that is being invoked when the inbound transformation is triggered (ie: Insert, Update, Merge).
        /// </summary>
        /// <param name="entity">The current instance of data entity object.</param>
        /// <returns>The current or the newly created instance data entity object.</returns>
        TEntity Set(TEntity entity);
    }
}
