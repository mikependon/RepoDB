using RepoDb.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb
{
    /// <summary>
    /// A class used for manipulating class objects via expressions.
    /// </summary>
    public static partial class ClassExpression
    {
        #region GetEntitiesPropertyValues

        /// <summary>
        /// Gets the values of the property of the data entities (via expression).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entities.</typeparam>
        /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
        /// <param name="entities">The list of the data entities.</param>
        /// <param name="expression">The expression to be parsed.</param>
        /// <returns>The values of the property of the data entities.</returns>
        public static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
            Expression<Func<TEntity, object>> expression)
            where TEntity : class
        {
            var property = ExpressionExtension.GetProperty<TEntity>(expression);
            var propertyCache = PropertyCache.Get<TEntity>()
                .Where(p => p.PropertyInfo == property || string.Equals(p.PropertyInfo.Name, property.Name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            return GetEntitiesPropertyValues<TEntity, TResult>(entities, propertyCache);
        }

        /// <summary>
        /// Gets the values of the property of the data entities (via <see cref="Field"/> object).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entities.</typeparam>
        /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
        /// <param name="entities">The list of the data entities.</param>
        /// <param name="field">The name of the target property defined as <see cref="Field"/>.</param>
        /// <returns>The values of the property of the data entities.</returns>
        public static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
            Field field)
            where TEntity : class
        {
            var classProperty = PropertyCache.Get<TEntity>()
                .Where(p => string.Equals(p.PropertyInfo.Name, field.Name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            return GetEntitiesPropertyValues<TEntity, TResult>(entities, classProperty);
        }

        /// <summary>
        /// Gets the values of the property of the data entities (via property name).
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entities.</typeparam>
        /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
        /// <param name="entities">The list of the data entities.</param>
        /// <param name="propertyName">The name of the target property.</param>
        /// <returns>The values of the property of the data entities.</returns>
        public static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
            string propertyName)
            where TEntity : class
        {
            var classProperty = PropertyCache.Get<TEntity>()
                .Where(p => string.Equals(p.PropertyInfo.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
            return GetEntitiesPropertyValues<TEntity, TResult>(entities, classProperty);
        }

        /// <summary>
        /// Gets the values of the property of the data entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entities.</typeparam>
        /// <typeparam name="TResult">The result type of the extracted property.</typeparam>
        /// <param name="entities">The list of the data entities.</param>
        /// <param name="property">The target property.</param>
        /// <returns>The values of the property of the data entities.</returns>
        internal static IEnumerable<TResult> GetEntitiesPropertyValues<TEntity, TResult>(IEnumerable<TEntity> entities,
            ClassProperty property)
            where TEntity : class
        {
            return GetPropertyValuesCache<TEntity, TResult>.Do(entities, property);
        }

        private static class GetPropertyValuesCache<TEntity, TResult>
            where TEntity : class
        {
            private static ConcurrentDictionary<int, Func<TEntity, TResult>> m_cache =
                new ConcurrentDictionary<int, Func<TEntity, TResult>>();

            private static Func<TEntity, TResult> GetFunc(ClassProperty property)
            {
                // Expressions
                var obj = Expression.Parameter(typeof(TEntity), "obj");

                // Set the body
                var body = (Expression)Expression.Property(obj, property.PropertyInfo);

                // Convert if necessary
                if (property.PropertyInfo.PropertyType != typeof(TResult))
                {
                    body = Expression.Convert(body, typeof(TResult));
                }

                // Set the function value
                return Expression
                    .Lambda<Func<TEntity, TResult>>(body, obj)
                    .Compile();
            }

            private static void Guard(ClassProperty property)
            {
                // Check the presence
                if (property == null)
                {
                    throw new NullReferenceException("Property");
                }

                // Check the equality
                if (typeof(TEntity) != property.PropertyInfo.DeclaringType)
                {
                    throw new InvalidOperationException("The declaring type of the property is not equal to the target entity type.");
                }
            }

            public static IEnumerable<TResult> Do(IEnumerable<TEntity> entities,
                ClassProperty property)
            {
                // Guard first
                Guard(property);

                // Variables needed
                var func = (Func<TEntity, TResult>)null;
                var key = property.GetHashCode();

                // Get from the cache
                if (m_cache.TryGetValue(key, out func) == false)
                {
                    func = GetFunc(property);
                }

                // Extract the values
                if (entities?.Any() == true)
                {
                    foreach (var entity in entities)
                    {
                        yield return func(entity);
                    }
                }
            }
        }

        #endregion

        #region GetProperties

        /// <summary>
        /// Gets the properties of the class.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <returns>The properties of the class.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>()
            where TEntity : class
        {
            return GetPropertiesCache<TEntity>.Do();
        }

        private static class GetPropertiesCache<TEntity>
            where TEntity : class
        {
            private static Func<IEnumerable<ClassProperty>> m_func;

            private static Func<IEnumerable<ClassProperty>> GetFunc()
            {
                var body = Expression.Constant(DataEntityExtension.GetProperties<TEntity>());
                return Expression
                    .Lambda<Func<IEnumerable<ClassProperty>>>(body)
                    .Compile();
            }

            public static IEnumerable<ClassProperty> Do()
            {
                if (m_func == null)
                {
                    m_func = GetFunc();
                }
                return m_func();
            }
        }

        #endregion

        #region GetPropertiesAndValues

        /// <summary>
        /// Extract the class properties and values and returns an enumerable of <see cref="PropertyValue"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the class.</typeparam>
        /// <param name="obj">The object to be extracted.</param>
        /// <returns>A list of <see cref="PropertyValue"/> object with extracted values.</returns>
        public static IEnumerable<PropertyValue> GetPropertiesAndValues<TEntity>(TEntity obj)
            where TEntity : class
        {
            return GetPropertiesValuesCache<TEntity>.Do(obj);
        }

        private static class GetPropertiesValuesCache<TEntity>
            where TEntity : class
        {
            private static Func<TEntity, IEnumerable<PropertyValue>> m_func;

            private static Func<TEntity, IEnumerable<PropertyValue>> GetFunc()
            {
                // Expressions
                var addMethod = typeof(List<PropertyValue>).GetMethod("Add", new[] { typeof(PropertyValue) });
                var obj = Expression.Parameter(typeof(TEntity), "obj");
                var constructor = typeof(PropertyValue).GetConstructor(new[]
                {
                    typeof(string),
                    typeof(object),
                    typeof(ClassProperty)
                });

                // Set the body
                var properties = PropertyCache.Get<TEntity>();
                var body = Expression.ListInit(
                    Expression.New(typeof(List<PropertyValue>)),
                    properties.Select(property =>
                    {
                        var name = Expression.Constant(property.GetMappedName());
                        var value = Expression.Convert(Expression.Property(obj, property.PropertyInfo), typeof(object));
                        var propertyValue = Expression.New(constructor,
                            name,
                            value,
                            Expression.Constant(property));
                        return Expression.ElementInit(addMethod, propertyValue);
                    }));

                // Set the function value
                return Expression
                    .Lambda<Func<TEntity, IEnumerable<PropertyValue>>>(body, obj)
                    .Compile();
            }

            public static IEnumerable<PropertyValue> Do(TEntity obj)
            {
                if (m_func == null)
                {
                    m_func = GetFunc();
                }
                return m_func(obj);
            }
        }

        #endregion
    }
}
