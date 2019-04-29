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
        #region GetProperties

        /// <summary>
        /// Gets the properties of the class.
        /// </summary>
        /// <typeparam name="TEntity">The target type.</typeparam>
        /// <returns>The properties of the class.</returns>
        public static IEnumerable<ClassProperty> GetProperties<TEntity>()
            where TEntity : class
        {
            return GetPropertiesExtractor<TEntity>.Extract();
        }

        /// <summary>
        /// Gets a function used to extract the properties of a class.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <returns>The properties of the class.</returns>
        private static Func<IEnumerable<ClassProperty>> GetCompiledFunctionForGetProperties<T>()
            where T : class
        {
            // Variables type (Command.None is preloaded everywhere)
            var properties = DataEntityExtension.GetProperties<T>();

            // Set the body
            var body = Expression.Constant(properties);

            // Set the function value
            return Expression
                .Lambda<Func<IEnumerable<ClassProperty>>>(body)
                .Compile();
        }

        private static class GetPropertiesExtractor<T>
            where T : class
        {
            private static readonly Func<IEnumerable<ClassProperty>> m_func;

            static GetPropertiesExtractor()
            {
                m_func = GetCompiledFunctionForGetProperties<T>();
            }

            public static IEnumerable<ClassProperty> Extract()
            {
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
            return ClassPropertiesValuesExtractor<TEntity>.Extract(obj);
        }

        /// <summary>
        /// Extract the class properties and values and returns an enumerable of <see cref="PropertyValue"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the class.</typeparam>
        /// <param name="obj">The object to be extracted.</param>
        /// <returns>A list of <see cref="PropertyValue"/> object with extracted values.</returns>
        [Obsolete("Use the 'GetPropertiesAndValues' method.")]
        public static IEnumerable<PropertyValue> Extract<TEntity>(TEntity obj)
            where TEntity : class
        {
            return ClassPropertiesValuesExtractor<TEntity>.Extract(obj);
        }

        /// <summary>
        /// Gets a function that returns the list of property values of the class.
        /// </summary>
        /// <param name="properties">The list of properties.</param>
        /// <returns>The enumerable value of class property values.</returns>
        private static Func<T, IEnumerable<PropertyValue>> GetCompiledFunctionForClassPropertiesValuesExtractor<T>(IEnumerable<ClassProperty> properties)
            where T : class
        {
            // Expressions
            var addMethod = typeof(List<PropertyValue>).GetMethod("Add", new[] { typeof(PropertyValue) });
            var obj = Expression.Parameter(typeof(T), "obj");
            var propertyValueConstructor = typeof(PropertyValue).GetConstructor(new[]
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
                    var propertyValue = Expression.New(propertyValueConstructor,
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

        private static class ClassPropertiesValuesExtractor<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertiesValuesExtractor()
            {
                m_func = GetCompiledFunctionForClassPropertiesValuesExtractor<T>(PropertyCache.Get<T>());
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion
    }
}
