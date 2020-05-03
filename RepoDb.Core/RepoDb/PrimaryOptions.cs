using System;
using System.Data;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    internal class PrimaryOptions<T> : IPrimaryOptions<T>
        where T : class
    {
        #region Privates

        private readonly Expression<Func<T, object>> m_expression;

        #endregion

        public PrimaryOptions(Expression<Func<T, object>> expression)
        {
            m_expression = expression;
        }

        #region Methods

        /*
         * Column
         */

        public IPrimaryOptions<T> Column(string column)
        {
            return Column(column, false);
        }

        public IPrimaryOptions<T> Column(string column,
            bool force)
        {
            PropertyMapper.Add<T>(m_expression, column, force);
            return this;
        }

        /*
         * DbType
         */

        public IPrimaryOptions<T> DbType(DbType dbType)
        {
            return DbType(dbType, false);
        }

        public IPrimaryOptions<T> DbType(DbType dbType,
            bool force)
        {
            TypeMapper.Add<T>(m_expression, dbType, force);
            return this;
        }

        #endregion
    }
}