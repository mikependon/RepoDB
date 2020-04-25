using System;
using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb
{ 
    public class ClassMap<T> where T : class
    {
        public Type EntityType { get; }

        public ClassMap()
        {
            EntityType = typeof(T);
        }
        
        protected ClassMap<T> Table(string table)
        {
            ClassMapper.Add<T>(table);
            return this;
        }
        
        protected IIdentityOptions<T> Id(Expression<Func<T, object>> expression)
        {
            IdentityMapper.Add<T>(expression);
            return new IdentityOptions<T>(expression);
        }

        protected IPrimaryOptions<T> Primary(Expression<Func<T, object>> expression)
        {
            PrimaryMapper.Add<T>(expression);
            return new PrimaryOptions<T>(expression);
        }

        protected IPropertyOptions<T> Map(Expression<Func<T, object>> expression)
        {
            return new PropertyOptions<T>(expression);
        }
    }
}