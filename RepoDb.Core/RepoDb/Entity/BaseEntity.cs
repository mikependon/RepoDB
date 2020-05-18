using System;
using System.Linq.Expressions;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class BaseEntity<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly IPropertyMap<TEntity> PropertyMap = new EntityPropertyMap<TEntity>();

        static BaseEntity()
        {
            typeof(TEntity).GetProperties().ForEach(x => PropertyMap.Map(x));
        }

        /// <summary>
        /// Map property by expression.
        /// </summary>
        /// <typeparam name="TProperty">type of the property</typeparam>
        /// <param name="expression">mapping expression</param>
        /// <returns>mapper</returns>
        public IConverter Map<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return PropertyMap.Map(expression);
        }
    }
}
