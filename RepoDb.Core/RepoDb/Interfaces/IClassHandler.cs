using RepoDb.Options;

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
        /// The method that is being invoked when the outbound execution is triggered (i.e.: BatchQuery, ExecuteQuery and Query).
        /// </summary>
        /// <param name="entity">The current instance of data entity object.</param>
        /// <param name="options">The instance of <see cref="ClassHandlerGetOptions"/> object in used during the hydration process.</param>
        /// <returns>The current or the newly created instance data entity object.</returns>
        TEntity Get(TEntity entity,
            ClassHandlerGetOptions options);

        /// <summary>
        /// The method that is being invoked when the inbound execution is triggered (i.e.: Insert, Update and Merge).
        /// </summary>
        /// <param name="entity">The current instance of data entity object.</param>
        /// <param name="options">The instance of <see cref="ClassHandlerSetOptions"/> object in used during the push operations.</param>
        /// <returns>The current or the newly created instance data entity object.</returns>
        TEntity Set(TEntity entity,
            ClassHandlerSetOptions options);
    }
}
