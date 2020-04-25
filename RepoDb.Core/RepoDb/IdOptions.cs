using System;
using System.Linq.Expressions;

namespace RepoDb
{
    internal class IdOptions<T> : IIdOptions<T> where T : class
    {
        private readonly Expression<Func<T, object>> m_expression;

        public IdOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }

        public IIdOptions<T> Column(string column)
        {
            PropertyMapper.Add(m_expression, column);
            return this;
        }
    }
}