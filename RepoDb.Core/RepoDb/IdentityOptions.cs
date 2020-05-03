using System;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    internal class IdentityOptions<T> : IIdentityOptions<T>
        where T : class
    {
        #region Privates

        private readonly Expression<Func<T, object>> m_expression;

        #endregion

        public IdentityOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }

        #region Methods

        public IIdentityOptions<T> Column(string column)
        {
            PropertyMapper.Add(m_expression, column);
            return this;
        }

        public IIdentityOptions<T> Column(string column,
            bool force)
        {
            PropertyMapper.Add(m_expression, column, force);
            return this;
        }

        #endregion
    }
}