using System;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that is used to define a data entity level mapping (ie: Primary, Identity, Table, Column, DB Type and Property Handler).
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    public class ClassMap<TEntity>
        where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the type of the target data entity.
        /// </summary>
        public Type EntityType { get; } = typeof(TEntity);

        #endregion

        #region Methods

        /*
         * Table
         */

        /// <summary>
        /// Maps the equivalent database object to be used (ie: Table or View).
        /// </summary>
        /// <param name="databaseObjectName">The target database object name.</param>
        /// <returns>The current instance.</returns>
        protected ClassMap<TEntity> Table(string databaseObjectName)
        {
            return Table(databaseObjectName, false);
        }

        /// <summary>
        /// Maps the equivalent database object to be used (ie: Table or View).
        /// </summary>
        /// <param name="table">The target database object name.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        protected ClassMap<TEntity> Table(string table,
            bool force)
        {
            ClassMapper.Add<TEntity>(table, force);
            return this;
        }

        /*
         * Identity
         */

        /// <summary>
        /// Maps the identity property (and define the mappings) to be used by the target data entity.
        /// </summary>
        /// <param name="expression">The expression that defines the identity property.</param>
        /// <returns>The current instance.</returns>
        protected IIdentityOptions<TEntity> Identity(Expression<Func<TEntity, object>> expression)
        {
            IdentityMapper.Add<TEntity>(expression);
            return new IdentityOptions<TEntity>(expression);
        }

        /*
         * Primary
         */

        /// <summary>
        /// Maps the primary property (and define the mappings) to be used by the target data entity.
        /// </summary>
        /// <param name="expression">The expression that defines the primary property.</param>
        /// <returns>The current instance.</returns>
        protected IPrimaryOptions<TEntity> Primary(Expression<Func<TEntity, object>> expression)
        {
            PrimaryMapper.Add<TEntity>(expression);
            return new PrimaryOptions<TEntity>(expression);
        }

        /*
         * Property
         */

        /// <summary>
        /// Define the property mappings to be used by the target data entity.
        /// </summary>
        /// <param name="expression">The expression that defines the primary property.</param>
        /// <returns>The current instance.</returns>
        protected IPropertyOptions<TEntity> Property(Expression<Func<TEntity, object>> expression)
        {
            return new PropertyOptions<TEntity>(expression);
        }

        #endregion
    }
}