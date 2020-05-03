using System;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{
    public class ClassMap<T> where T : class
    {
        #region Properties

        public Type EntityType { get; } = typeof(T);

        #endregion

        #region Methods

        /*
         * Table
         */

        protected ClassMap<T> Table(string table)
        {
            return Table(table, false);
        }

        protected ClassMap<T> Table(string table,
            bool force)
        {
            ClassMapper.Add<T>(table, force);
            return this;
        }

        /*
         * Identity
         */

        protected IIdentityOptions<T> Identity(Expression<Func<T, object>> expression)
        {
            IdentityMapper.Add<T>(expression);
            return new IdentityOptions<T>(expression);
        }

        /*
         * Primary
         */
        protected IPrimaryOptions<T> Primary(Expression<Func<T, object>> expression)
        {
            PrimaryMapper.Add<T>(expression);
            return new PrimaryOptions<T>(expression);
        }

        /*
         * Property
         */

        protected IPropertyOptions<T> Property(Expression<Func<T, object>> expression)
        {
            return new PropertyOptions<T>(expression);
        }

        #endregion
    }
}