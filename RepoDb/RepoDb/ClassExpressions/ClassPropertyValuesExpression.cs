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
        /// <summary>
        /// Extracts the class properties and values and returns an enumerable of <see cref="PropertyValue"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the class.</typeparam>
        /// <param name="obj">The object to be extracted.</param>
        /// <returns>A list of <see cref="PropertyValue"/> object with extracted values.</returns>
        public static IEnumerable<PropertyValue> GetPropertiesAndValues<TEntity>(TEntity obj)
            where TEntity : class
        {
            return ClassPropertyValuesExtractor<TEntity>.Extract(obj);
        }

        /// <summary>
        /// Extracts the class properties and values and returns an enumerable of <see cref="PropertyValue"/> object.
        /// </summary>
        /// <typeparam name="TEntity">The target type of the class.</typeparam>
        /// <param name="obj">The object to be extracted.</param>
        /// <returns>A list of <see cref="PropertyValue"/> object with extracted values.</returns>
        [Obsolete("Use the 'GetPropertiesAndValues' method.")]
        public static IEnumerable<PropertyValue> Extract<TEntity>(TEntity obj)
            where TEntity : class
        {
            return ClassPropertyValuesExtractor<TEntity>.Extract(obj);
        }

        #region GetPropertiesAndValues<T> Functions

        /// <summary>
        /// Gets a function that returns the list of property values of the class.
        /// </summary>
        /// <param name="properties">The list of properties.</param>
        /// <returns>The enumerable value of class property values.</returns>
        private static Func<T, IEnumerable<PropertyValue>> GetCompiledFunctionForClassPropertyValuesExtractor<T>(IEnumerable<ClassProperty> properties)
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

        #region ClassPropertyValuesExtractor<T>

        private static class ClassPropertyValuesExtractor<T> where T : class
        {
            private static readonly Func<T, IEnumerable<PropertyValue>> m_func;

            static ClassPropertyValuesExtractor()
            {
                m_func = GetCompiledFunctionForClassPropertyValuesExtractor<T>(PropertyCache.Get<T>());
            }

            public static IEnumerable<PropertyValue> Extract(T obj)
            {
                return m_func(obj);
            }
        }

        #endregion

        #endregion
    }
}
