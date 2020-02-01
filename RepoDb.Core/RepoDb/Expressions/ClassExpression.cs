using RepoDb.Extensions;
using System;
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
        #region GetPropertyValues

        /// <summary>
        /// Gets the values of the property of the data entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entities.</typeparam>
        /// <param name="entities">The list of the data entities.</param>
        /// <param name="property">The target property.</param>
        /// <returns>The values of the property of the data entities.</returns>
        public static IEnumerable<object> GetPropertyValue<TEntity>(IEnumerable<TEntity> entities,
            ClassProperty property = null)
            where TEntity : class
        {
            return GetPropertyValuesCache<TEntity>.Do(entities, property);
        }

        private static class GetPropertyValuesCache<T>
            where T : class
        {
            private static Func<T, object> m_func;

            private static Func<T, object> GetFunc(ClassProperty property)
            {
                // Expressions
                var obj = Expression.Parameter(typeof(T), "obj");

                // Set the body
                var body = (Expression)Expression.Property(obj, property.PropertyInfo);

                // Convert if necessary
                if (property.PropertyInfo.PropertyType != typeof(object))
                {
                    body = Expression.Convert(body, typeof(object));
                }
                
                // Set the function value
                return Expression
                    .Lambda<Func<T, object>>(body, obj)
                    .Compile();
            }

            public static IEnumerable<object> Do(IEnumerable<T> entities,
                ClassProperty property)
            {
                if (m_func == null)
                {
                    m_func = GetFunc(property);
                }
                if (entities?.Any() == true)
                {
                    foreach (var entity in entities)
                    {
                        yield return m_func(entity);
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

        private static class GetPropertiesCache<T>
            where T : class
        {
            private static Func<IEnumerable<ClassProperty>> m_func;

            private static Func<IEnumerable<ClassProperty>> GetFunc()
            {
                var body = Expression.Constant(DataEntityExtension.GetProperties<T>());
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

        private static class GetPropertiesValuesCache<T>
            where T : class
        {
            private static Func<T, IEnumerable<PropertyValue>> m_func;

            private static Func<T, IEnumerable<PropertyValue>> GetFunc(IEnumerable<ClassProperty> properties)
            {
                // Expressions
                var addMethod = typeof(List<PropertyValue>).GetMethod("Add", new[] { typeof(PropertyValue) });
                var obj = Expression.Parameter(typeof(T), "obj");
                var constructor = typeof(PropertyValue).GetConstructor(new[]
                {
                    typeof(string),
                    typeof(object),
                    typeof(ClassProperty)
                });

                // Set the body
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
                    .Lambda<Func<T, IEnumerable<PropertyValue>>>(body, obj)
                    .Compile();
            }

            public static IEnumerable<PropertyValue> Do(T obj)
            {
                if (m_func == null)
                {
                    m_func = GetFunc(PropertyCache.Get<T>());
                }
                return m_func(obj);
            }
        }

        #endregion
    }
}
