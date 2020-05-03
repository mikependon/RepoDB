using System;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that implements the identity mapping options.
    /// </summary>
    /// <typeparam name="TEntity">The type of the data entity.</typeparam>
    internal class IdentityOptions<TEntity> : IIdentityOptions<TEntity>
        where TEntity : class
    {
        #region Privates

        private readonly Expression<Func<TEntity, object>> m_expression;

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="IdentityOptions{TEntity}"/> class.
        /// </summary>
        /// <param name="expression">The expression that defines the identity property.</param>
        public IdentityOptions(Expression<Func<TEntity, object>> expression)
        {
            m_expression = expression;
        }

        #region Methods

        /// <summary>
        /// Maps the equivalent database column of the current identity property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <returns>The current instance.</returns>
        public IIdentityOptions<TEntity> Column(string column)
        {
            PropertyMapper.Add(m_expression, column);
            return this;
        }

        /// <summary>
        /// Maps the equivalent database column of the current identity property.
        /// </summary>
        /// <param name="column">The name of the database column.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        /// <returns>The current instance.</returns>
        public IIdentityOptions<TEntity> Column(string column,
            bool force)
        {
            PropertyMapper.Add(m_expression, column, force);
            return this;
        }

        #endregion
    }
}