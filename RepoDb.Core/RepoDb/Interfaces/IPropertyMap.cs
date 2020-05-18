using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Interfaces
{
    /// <summary>
    /// An interface used to map a property converter.
    /// </summary>
    public interface IPropertyMap<TEntity> where TEntity : class
    {
        /// <summary>
        /// Map property info.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        IConverter Map(PropertyInfo propertyInfo);

        /// <summary>
        /// Map entity expression.
        /// </summary>
        /// <typeparam name="TProperty">property type</typeparam>
        /// <returns>sql property</returns>
        /// <returns></returns>
        IConverter Map<TProperty>(Expression<Func<TEntity, TProperty>> expression);
    }
}
