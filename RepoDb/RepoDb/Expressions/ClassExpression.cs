using RepoDb.Extensions;
using RepoDb.Interfaces;
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
        #region GetProperties

        /// <summary>
        /// Gets the properties of the class.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>The properties of the class.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>(IDbSetting dbSetting)
            where TEntity : class
        {
            return GetPropertiesCache<TEntity>.Do(dbSetting);
        }

        private static class GetPropertiesCache<T>
            where T : class
        {
            private static Func<IEnumerable<ClassProperty>> m_func;

            private static Func<IEnumerable<ClassProperty>> GetFunc(IDbSetting dbSetting)
            {
                var body = Expression.Constant(DataEntityExtension.GetProperties<T>(dbSetting));
                return Expression
                    .Lambda<Func<IEnumerable<ClassProperty>>>(body)
                    .Compile();
            }

            public static IEnumerable<ClassProperty> Do(IDbSetting dbSetting)
            {
                if (m_func == null)
                {
                    m_func = GetFunc(dbSetting);
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
        /// <param name="dbSetting">The database setting that is currently in used.</param>
        /// <returns>A list of <see cref="PropertyValue"/> object with extracted values.</returns>
        public static IEnumerable<PropertyValue> GetPropertiesAndValues<TEntity>(TEntity obj,
            IDbSetting dbSetting)
            where TEntity : class
        {
            return GetPropertiesValuesCache<TEntity>.Do(obj, dbSetting);
        }

        private static class GetPropertiesValuesCache<TEntity>
            where TEntity : class
        {
            private static Func<TEntity, IEnumerable<PropertyValue>> m_func;

            private static Func<TEntity, IEnumerable<PropertyValue>> GetFunc(IEnumerable<ClassProperty> properties)
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
                var body = Expression.ListInit(
                    Expression.New(typeof(List<PropertyValue>)),
                    properties.Select(property =>
                    {
                        var name = Expression.Constant(property.GetUnquotedMappedName());
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

            public static IEnumerable<PropertyValue> Do(TEntity obj,
                IDbSetting dbSetting)
            {
                if (m_func == null)
                {
                    m_func = GetFunc(PropertyCache.Get<TEntity>(dbSetting));
                }
                return m_func(obj);
            }
        }

        #endregion
    }
}
