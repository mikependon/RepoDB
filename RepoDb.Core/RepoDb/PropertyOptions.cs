using System;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class PropertyOptions<T> : IPropertyOptions<T> where T : class
    {
        private readonly Expression<Func<T, object>> m_expression;

        public PropertyOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }
        
        public IPropertyOptions<T> Column(string column)
        {
            PropertyMapper.Add<T>(m_expression, column);
            return this;
        }
        
        public IPropertyOptions<T> DbType(DbType dbType)
        {
            TypeMapper.Add<T>(m_expression, dbType);
            return this;
        }

        public IPropertyOptions<T> PropertyHandler<THandler>(THandler propertyHandler)
        {
            PropertyHandlerMapper.Add(m_expression, propertyHandler);
            return this;
        }
    }
}