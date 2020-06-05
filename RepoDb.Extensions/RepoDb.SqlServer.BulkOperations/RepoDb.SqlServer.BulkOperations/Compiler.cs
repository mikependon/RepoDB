using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace RepoDb.SqlServer.BulkOperations
{
    /// <summary>
    /// An internal compiler class used to compile necessary expressions that is needed to enhance the code execution.
    /// </summary>
    internal static class Compiler
    {

        #region GetEntitiesPropertyValues

        /// <summary>
        /// Gets a compiled setter function for a data entity property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <returns>The compiled function to set the value of the property.</returns>
        public static Action<TEntity, object> GetPropertySetterFunc<TEntity>(string propertyName)
            where TEntity : class
        {
            return GetEntitiesPropertyValuesCache<TEntity>.GetFunc(PropertyCache.Get<TEntity>(propertyName));
        }

        private static class GetEntitiesPropertyValuesCache<TEntity>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Action<TEntity, object>> m_cache =
                new ConcurrentDictionary<int, Action<TEntity, object>>();

            public static Action<TEntity, object> GetFunc(ClassProperty property)
            {
                var func = (Action<TEntity, object>)null;

                // Try get from cache
                if (m_cache.TryGetValue(property.GetHashCode(), out func) == false)
                {
                    // Parameters
                    var entity = Expression.Parameter(typeof(TEntity), "entity");
                    var value = Expression.Parameter(typeof(object), "value");

                    // Set the body
                    var converted = Expression.Convert(value, property.PropertyInfo.PropertyType);
                    var body = (Expression)Expression.Call(entity, property.PropertyInfo.SetMethod, converted);

                    // Set the function value
                    func = Expression
                        .Lambda<Action<TEntity, object>>(body, entity, value)
                        .Compile();
                }

                // return the function
                return func;
            }
        }

        #endregion

    }
}
