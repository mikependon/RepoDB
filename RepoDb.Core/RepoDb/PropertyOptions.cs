using System;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    internal class PropertyOptions<T> : IPropertyOptions<T>
        where T : class
    {
        #region Privates

        private readonly Expression<Func<T, object>> m_expression;

        #endregion

        public PropertyOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }

        #region Methods

        /*
         * Column
         */

        public IPropertyOptions<T> Column(string column)
        {
            return Column(column, false);
        }

        public IPropertyOptions<T> Column(string column,
            bool force)
        {
            PropertyMapper.Add<T>(m_expression, column, force);
            return this;
        }

        /*
         * DbType
         */

        public IPropertyOptions<T> DbType(DbType dbType)
        {
            return DbType(dbType, false);
        }

        public IPropertyOptions<T> DbType(DbType dbType,
            bool force)
        {
            TypeMapper.Add<T>(m_expression, dbType, force);
            return this;
        }

        /*
         * PropertyHandler
         */

        public IPropertyOptions<T> PropertyHandler<TPropertyHandler>()
        {
            return PropertyHandler<TPropertyHandler>(false);
        }

        public IPropertyOptions<T> PropertyHandler<TPropertyHandler>(bool force)
        {
            return PropertyHandler<TPropertyHandler>(Activator.CreateInstance<TPropertyHandler>(), force);
        }

        public IPropertyOptions<T> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler)
        {
            return PropertyHandler<TPropertyHandler>(propertyHandler, false);
        }

        public IPropertyOptions<T> PropertyHandler<TPropertyHandler>(TPropertyHandler propertyHandler,
            bool force)
        {
            PropertyHandlerMapper.Add(m_expression, propertyHandler);
            return this;
        }

        #endregion
    }
}