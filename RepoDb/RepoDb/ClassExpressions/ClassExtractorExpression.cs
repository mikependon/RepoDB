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
        /// Extracts a dynamic object value to become an enumerable of objects.
        /// </summary>
        /// <param name="obj">The object to be extracted.</param>
        /// <returns>The extracted values.</returns>
        private static IEnumerable<object> Extract<T>(T obj)
        {
            return Extractor<T>.Extract(obj);
        }

        /// <summary>
        /// An extractor class for caching.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        private static class Extractor<T>
        {
            private static readonly Func<T, IEnumerable<object>> m_func;

            static Extractor()
            {
                // Set the value
                var type = typeof(T);
                var properties = type.GetProperties();

                // Expressions
                var listType = typeof(List<object>);
                var method = listType.GetMethod("Add", new[] { typeof(object) });
                var newList = Expression.New(listType);
                var param = Expression.Parameter(typeof(T), "obj");

                // Set the body
                var body = Expression.ListInit(
                    newList,
                    properties.Select(property =>
                    {
                        return Expression.ElementInit(method,
                            Expression.Convert(Expression.Property(param, property), typeof(object)));
                    }));

                // Set the function value
                m_func = Expression
                    .Lambda<Func<T, IEnumerable<object>>>(body, param)
                    .Compile();
            }

            /// <summary>
            /// Extracts a dynamic object value to become an enumerable of objects.
            /// </summary>
            /// <param name="obj">The object to be extracted.</param>
            /// <returns>The extracted values.</returns>
            public static IEnumerable<object> Extract(T obj)
            {
                return m_func(obj);
            }
        }
    }
}
