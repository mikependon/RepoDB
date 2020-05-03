using System;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that implements the property mapping options.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal class PropertyOptions<TEntity> : IPropertyOptions<TEntity>
        where TEntity : class
    {
        #region Privates

        private readonly Expression<Func<TEntity, object>> m_expression;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="PrimaryOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="expression">The expression that defines the target property.</param>
        public PropertyOptions(Expression<Func<TEntity, object>> expression)
        {
            m_expression = expression;
        }

        #region Methods

        /*
         * Column
         */

        /// <summary>
        /// Maps the equivalent database column of the current target property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> Column(string column)
        {
            return Column(column, false);
        }

        /// <summary>
        /// Maps the equivalent database column of the current target property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> Column(string column,
            bool force)
        {
            PropertyMapper.Add<TEntity>(m_expression, column, force);
            return this;
        }

        /*
         * DbType
         */

        /// <summary>
        /// Maps the equivalent database type of the current target property.
        /// </summary>
        /// <param name="dbType">The target database type.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> DbType(DbType dbType)
        {
            return DbType(dbType, false);
        }

        /// <summary>
        /// Maps the equivalent database type of the current target property.
        /// </summary>
        /// <param name="dbType">The target database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> DbType(DbType dbType,
            bool force)
        {
            TypeMapper.Add<TEntity>(m_expression, dbType, force);
            return this;
        }

        /*
         * PropertyHandler
         */

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>()
        {
            return PropertyHandler<TPropertyHandler>(false);
        }

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>(bool force)
        {
            return PropertyHandler<TPropertyHandler>(Activator.CreateInstance<TPropertyHandler>(), force);
        }

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the handler.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler)
        {
            return PropertyHandler<TPropertyHandler>(propertyHandler, false);
        }

        /// <summary>
        /// Maps the property handler to be used by the current target property.
        /// </summary>
        /// <typeparam name="TPropertyHandler">The type of the handler.</typeparam>
        /// <param name="propertyHandler">The instance of the handler.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public IPropertyOptions<TEntity> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force)
        {
            PropertyHandlerMapper.Add(m_expression, propertyHandler);
            return this;
        }

        #endregion
    }
}