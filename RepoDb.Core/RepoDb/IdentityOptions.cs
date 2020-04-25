using System;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    internal class IdentityOptions<T> : IIdentityOptions<T> where T : class
    {
        private readonly Expression<Func<T, object>> m_expression;

        public IdentityOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }

        public IIdentityOptions<T> Column(string column)
        {
            PropertyMapper.Add(m_expression, column);
            return this;
        }
    }
}