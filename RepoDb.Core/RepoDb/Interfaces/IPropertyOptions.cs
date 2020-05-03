using System.Data;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to mark a class to be a property handler mapping options.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public interface IPropertyOptions<TEntity> where TEntity : class
    {
        /*
         * Column
         */

        /// <summary>
        /// Maps the equivalent database column of the current target property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> Column(string column);

        /// <summary>
        /// Maps the equivalent database column of the current target property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> Column(string column,
            bool force);

        /*
         * DbType
         */

        /// <summary>
        /// Maps the equivalent database type of the current target property.
        /// </summary>
        /// <param name="dbType">The target database type.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> DbType(DbType dbType);

        /// <summary>
        /// Maps the equivalent database type of the current target property.
        /// </summary>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> DbType(DbType dbType,
            bool force);

        /*
         * PropertyHandler
         */

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>();

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>(bool force);

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the handler.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler);

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force);
    }
}