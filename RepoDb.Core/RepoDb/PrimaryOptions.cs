using System;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    internal class PrimaryOptions<T> : IPrimaryOptions<T> where T : class
    {
        private readonly Expression<Func<T, object>> m_expression;

        public PrimaryOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }
        
        public IPrimaryOptions<T> Column(string column)
        {
            PropertyMapper.Add<T>(m_expression, column);
            return this;
        }
        
        public IPrimaryOptions<T> DbType(DbType dbType)
        {
            TypeMapper.Add<T>(m_expression, dbType);
            return this;
        }
    }
}